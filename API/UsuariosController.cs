using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;
using Grandes_Amigos.Services;

namespace Grandes_Amigos.Api;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Usuarios")]
public class UsuariosController : Controller
{
    // UsuariosController.cs
    // ```
    // CRUD de Usuarios
    // Patrón: [host]/Api/Usuarios
    private readonly ILogger<UsuariosController> _logger;
    private readonly IConfiguration Config;
    private ContextoDb Contexto;
    private readonly EmailService emailService;

    public UsuariosController(
        ILogger<UsuariosController> logger,
        ContextoDb contexto,
        IConfiguration ajustes,
        EmailService email
    )
    {
        _logger = logger;
        Contexto = contexto;
        Config = ajustes;
        emailService = email;
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("Todos")]
    [SwaggerOperation(
        Summary = "Lista todos los Usuarios.",
        Description = "Lee todas las entradas de la tabla `Usuarios`."
    )]
    [SwaggerResponse(200, "Hay al menos una entrada en la tabla `Usuarios`.")]
    [SwaggerResponse(204, "La tabla `Usuarios` está vacía.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    public IActionResult VerTodos()
    {
        List<Usuario> Usuarios = Contexto.Usuarios.ToList();
        return Ok(Usuarios);
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna un Usuario.",
        Description = "Toma el ID del Usuario de la ruta; y si un Usuario con ése ID existe en la base de datos, lo lee."
    )]
    [SwaggerResponse(200, "El Usuario existe en la base de datos.")]
    [SwaggerResponse(404, "El Usuario no existe en la base de datos.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    public IActionResult DetallesUsuario([FromRoute] int id)
    {
        Usuario? UsuarioEncontrado = Contexto.Usuarios.Find(id);
        if (UsuarioEncontrado == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(UsuarioEncontrado);
        }
    }

    [AllowAnonymous] //Éste endpoint ahora es accedido por los usuarios para que éstos creen una cuenta.
    [HttpPost("Nuevo")]
    [SwaggerOperation(
        Summary = "Crea un nuevo Usuario.",
        Description = "Crea una entrada en la tabla `Usuarios`, tomando el cuerpo del pedido como parámetro. Ve a `/Models/Usuario.cs` para saber qué entra aquí."
    )]
    [SwaggerResponse(201, "Se cargó el nuevo Usuario a la base de datos exitosamente.")]
    [SwaggerResponse(
        400,
        "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal."
    )]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult NuevoUsuario([FromForm] Usuario NuevoUsuario)
    {
        try
        {
            if (ModelState.IsValid)
            {
                NuevoUsuario.Clave = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: NuevoUsuario.Clave,
                        salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 4096,
                        numBytesRequested: 256 / 8
                    )
                );
                Contexto.Usuarios.Add(NuevoUsuario);
                Contexto.SaveChanges();
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
        Summary = "Borra un Usuario.",
        Description = "Borra una entrada de la tabla 'Usuarios' tomando el ID de la ruta como parámetro. Si el Usuario existe, es borrado."
    )]
    [SwaggerResponse(200, "El Usuario fué borrado con éxito.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(400, "El Usuario seleccionado no existe, es decir, el ID es inválido.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult BorrarUsuario([FromRoute] int id)
    {
        Usuario? UsuarioSeleccionado = Contexto.Usuarios.Find(id);
        if (UsuarioSeleccionado != null)
        {
            try
            {
                Contexto.Remove(UsuarioSeleccionado);
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
            return BadRequest("El Usuario seleccionado no existe.");
        }
    }

    [AllowAnonymous]
    [HttpPost("ClaveOlvidada")]
    [SwaggerOperation(
            Summary = "Permite al usuario restablecer su clave.",
            Description = "Genera un token de reinicio y envía un enlace al usuario conteniendo dicho token."
        )]
    [SwaggerResponse(200, "Se envió el correo de recuperación.")]
    [SwaggerResponse(400, "Se ingresó una dirección de correo no registrada.")]
    [SwaggerResponse(500, "Ocurrió un error.")]
    public async Task<IActionResult> ClaveOlvidada([FromForm] string correo)
    {
        try
        {
            Usuario? UsuarioSeleccionado = Contexto.Usuarios.FirstOrDefault(item =>
                item.Correo == correo
            );
            if (UsuarioSeleccionado == null)
            {
                return BadRequest("La cuenta pedida no existe.");
            }

            // Generar token sin hash (para el enlace)
            string TokenSinHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // Convertir el token a un hash para guardar en DB
            string TokenHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: TokenSinHash,
                    salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 100,
                    numBytesRequested: 64
                )
            );

            // Guardar token en la base (para usuario)
            Recuperación NuevoToken = new Recuperación
            {
                Token_Recuperación = TokenHash,
                ID_Usuario = UsuarioSeleccionado.NumDocumento,
                Válido_Hasta = DateTime.Now.AddMinutes(10),
                Rol = "user",
            };

            Contexto.Tokens.Add(NuevoToken);
            await Contexto.SaveChangesAsync();

            // Construir enlace de recuperación
            string link =
                $"http://127.0.0.1:5020/Api/Admin/RecuperarCuenta?TokenRecuperacion={Uri.EscapeDataString(TokenSinHash)}";

            string body =
                $@"
            <h1>Saludos</h1>
            <p>{UsuarioSeleccionado.Nombre}, recibiste este correo porque hubo una solicitud para recuperar tu clave. 
            Si fuiste vos, <a href='{link}'>haz clic aquí</a> para cambiar tu contraseña.</p>
            <h4>El enlace es válido por 10 minutos.</h4>";

            // ✅ Usar el EmailService
            await emailService.SendEmailAsync(
                UsuarioSeleccionado.Correo,
                "Reinicio de contraseña",
                body
            );
            return Ok("Revisa tu correo, allí recibirás el enlace de recuperación.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ClaveOlvidada");
            return StatusCode(500, "Error interno al enviar el correo.");
        }
    }

    [AllowAnonymous]
    [HttpGet("RecuperarCuenta")]
    [SwaggerOperation(
        Summary = "Autoriza al usuario a cambiar su clave.",
        Description = "Revisa si el enlace tiene un token válido, y si lo es, permite el cambio de contraseña."
    )]
    [SwaggerResponse(400, "El token está vacío o es inválido.")]
    public IActionResult RecuperarCuenta([FromQuery(Name = "TokenRecuperacion")] string token) {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token vacío.");
        }

        // recalcular hash, igual que en ClaveOlvidada
        string TokenHash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: token,
                salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 100,
                numBytesRequested: 64
            )
        );

        var tokenRow = Contexto.Tokens.Find(TokenHash);
        if (tokenRow == null || DateTime.Now >= tokenRow.Válido_Hasta)
        {
            return BadRequest("Token inválido o expirado.");
        }

        // redirigir al formulario estático con el token
        string url = $"/recuperar.html?TokenRecuperacion={Uri.EscapeDataString(token)}";
        return Redirect(url);
    }
}