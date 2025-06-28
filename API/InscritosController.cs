using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Inscritos")]
public class InscritosController : Controller
{
    // InscritosController.cs
    // ```
    // CRUD de Inscritos
    // Patrón: [host]/Api/Inscritos
    private readonly ILogger<InscritosController> _logger;
    private readonly IConfiguration Config;
    private ContextoDb Contexto;

    public InscritosController(
        ILogger<InscritosController> logger,
        ContextoDb contexto,
        IConfiguration ajustes
    )
    {
        _logger = logger;
        Contexto = contexto;
        Config = ajustes;
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("Todos")]
    [SwaggerOperation(
        Summary = "Lista todos los inscritos.",
        Description = "Lee todas las entradas de la tabla `Inscritos`."
    )]
    [SwaggerResponse(200, "Hay al menos una entrada en la tabla `Inscritos`.")]
    [SwaggerResponse(204, "La tabla `Inscritos` está vacía.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    public IActionResult VerTodos()
    {
        List<Inscrito> inscritos = Contexto.Inscritos.ToList();
        return Ok(inscritos);
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna un inscrito.",
        Description = "Toma el ID del inscrito de la ruta; y si un inscrito con ése ID existe en la base de datos, lo lee."
    )]
    [SwaggerResponse(200, "El inscrito existe en la base de datos.")]
    [SwaggerResponse(404, "El inscrito no existe en la base de datos.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    public IActionResult DetallesInscrito([FromRoute] int id)
    {
        Inscrito? InscritoEncontrado = Contexto.Inscritos.Find(id);
        if (InscritoEncontrado == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(InscritoEncontrado);
        }
    }

    [AllowAnonymous] //Éste endpoint ahora es accedido por los usuarios para que éstos creen una cuenta.
    [HttpPost("Nuevo")]
    [SwaggerOperation(
        Summary = "Crea un nuevo inscrito.",
        Description = "Crea una entrada en la tabla `Inscritos`, tomando el cuerpo del pedido como parámetro. Ve a `/Models/Inscrito.cs` para saber qué entra aquí."
    )]
    [SwaggerResponse(201, "Se cargó el nuevo inscrito a la base de datos exitosamente.")]
    [SwaggerResponse(
        400,
        "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal."
    )]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public async Task<IActionResult> NuevoInscrito([FromForm] Inscrito NuevoInscrito)
    {
        try
        {
            if (ModelState.IsValid)
            {
                NuevoInscrito.Clave = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: NuevoInscrito.Clave,
                        salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 4096,
                        numBytesRequested: 256 / 8
                    )
                );
                Contexto.Inscritos.Add(NuevoInscrito);
                await Contexto.SaveChangesAsync();
                return Created(); //Pendiente: Crear una vista que informe que la creación de la cuenta fué exitosa.
            }
            else
            {
                List<string> ErroresModelo = new List<string>();
                var errores = ModelState.Values.SelectMany(value => value.Errors);
                foreach (var item in errores)
                {
                    ErroresModelo.Add(item.ErrorMessage);
                }
                return BadRequest(
                    "Estado de modelo inválido:\n" + string.Join("\n", ErroresModelo)
                );
            }
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Authorize(Policy = "Ministerio")] //Pendiente: Implementar política separada para los usuarios.
    [HttpDelete("Borrar/{id}")]
    [SwaggerOperation(
        Summary = "Borra un inscrito.",
        Description = "Borra una entrada de la tabla 'Inscritos' tomando el ID de la ruta como parámetro. Si el inscrito existe, es borrado."
    )]
    [SwaggerResponse(200, "El inscrito fué borrado con éxito.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(400, "El inscrito seleccionado no existe, es decir, el ID es inválido.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult BorrarInscrito([FromRoute] int id)
    {
        Inscrito? InscritoSeleccionado = Contexto.Inscritos.Find(id);
        if (InscritoSeleccionado != null)
        {
            try
            {
                Contexto.Remove(InscritoSeleccionado);
                Contexto.SaveChanges();
                return Ok();
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, ex);
            }
        }
        else
        {
            return BadRequest("El inscrito seleccionado no existe.");
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [SwaggerOperation(
        Summary = "Inicia la sesión del inscrito.",
        Description = "Verifica documento y clave, y retorna 200 si es exitoso, junto con un JWT y los datos públicos del inscrito."
    )]
    [SwaggerResponse(200, "Login exitoso.")]
    [SwaggerResponse(400, "Documento o clave incorrectos.")]
    public async Task<IActionResult> IniciarSesiónInscrito([FromForm] LoginViewInscrito login)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos obligatorios.");

        // Buscar por documento
        var inscrito = await Contexto.Inscritos.FirstOrDefaultAsync(i =>
            i.NumDocumento == login.NumDocumento
        );

        if (inscrito == null)
            return BadRequest("Documento no registrado.");

        var saltString = Config["Salt"];
        if (string.IsNullOrEmpty(saltString))
            return StatusCode(500, "No se configuró el salt para el hash de contraseñas.");

        var claveHash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: login.Clave,
                salt: System.Text.Encoding.UTF8.GetBytes(saltString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 4096,
                numBytesRequested: 256 / 8
            )
        );

        if (claveHash != inscrito.Clave)
            return BadRequest("Clave incorrecta.");

        // Claims públicos (incluye DNI, no clave)
        var claims = new[]
        {
            new Claim("NumDocumento", inscrito.NumDocumento.ToString()),
            new Claim("TipoDocumento", inscrito.TipoDocumento ?? ""),
            new Claim("Correo", inscrito.Correo ?? ""),
            new Claim("Teléfono", inscrito.Teléfono ?? ""),
            new Claim("Asociación", inscrito.Asociación ?? ""),
            new Claim("Nombre", inscrito.Nombre ?? ""),
        };

        var jwtKey = Config["TokenAuthentication:SecretKey"] ?? Config["JwtKey"];
        if (string.IsNullOrEmpty(jwtKey))
            return StatusCode(500, "No se configuró la clave JWT.");

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Config["TokenAuthentication:Issuer"] ?? "GrandesAmigos",
            audience: Config["TokenAuthentication:Audience"] ?? "GrandesAmigos",
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: creds
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Devuelve token y datos públicos (incluye DNI, no clave)
        return Ok(
            new
            {
                token = tokenString,
                usuario = new
                {
                    NumDocumento = inscrito.NumDocumento,
                    TipoDocumento = inscrito.TipoDocumento,
                    Correo = inscrito.Correo,
                    Teléfono = inscrito.Teléfono,
                    Asociación = inscrito.Asociación,
                    Nombre = inscrito.Nombre,
                },
            }
        );
    }
}
