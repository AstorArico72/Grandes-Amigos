using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

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

    public UsuariosController(
        ILogger<UsuariosController> logger,
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
    public async Task<IActionResult> NuevoUsuario([FromForm] Usuario NuevoUsuario)
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
        Description = "Genera un token de reinicio, y envía un enlace al usuario conteniendo dicho token."
    )]
    //Éste endpoint tendría que ser llamado desde un botón "Clave olvidada" o algo así.
    public async Task<IActionResult> ClaveOlvidada([FromForm] string correo) {
        try
        {
            Usuario? UsuarioSeleccionado = await Contexto.Usuarios.FirstOrDefaultAsync(item => item.Correo == correo);
            if (UsuarioSeleccionado == null)
            {
                return BadRequest("La cuenta pedida no existe.");
            }

            //Ésto genera un token de reinicio. ¿Son 64 bytes suficiente?
            string TokenSinHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            //Ésto convierte el token a un hash.
            string TokenHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: TokenSinHash,
                    salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 100,
                    numBytesRequested: 64
                )
            );

            //Ésto sirve para guardar el token convertido a hash en la base de datos.
            Recuperación NuevoToken = new Recuperación();
            NuevoToken.Token_Recuperación = TokenHash;
            NuevoToken.ID_Usuario = UsuarioSeleccionado.NumDocumento;
            NuevoToken.Válido_Hasta = DateTime.Now.AddMinutes(10);
            Contexto.Tokens.Add(NuevoToken);

            //Ésto genera el correo de recuperación y lo envía.
            var Mensaje = new MimeKit.MimeMessage ();
            Mensaje.To.Add (new MailboxAddress (UsuarioSeleccionado.Nombre, UsuarioSeleccionado.Correo));
            Mensaje.From.Add (new MailboxAddress ("Grandes Amigos", Config["Correo:UsuarioSMTP"]));
            Mensaje.Subject = "Reinicio de contraseña";
            TextPart HtmlMensaje = new TextPart ("html"){
                //El enlace enviado por correo contiene el token sin convertir a hash, que después se coteja con el token convertido a hash en la BD.
                Text = @$"
                <h1>Saludos</h1>
                <p>{UsuarioSeleccionado.Nombre}, éste correo fue enviado porque hubo una solicitud para recuperar acceso a tu cuenta. Si tú hiciste ésa solicitud, <a href='https://127.0.0.1:5020/Api/Usuarios/RecuperarCuenta?TokenRecuperacion={TokenSinHash}'> entra aquí </a> para poder cambiar la clave.</p>
                <h4> El enlace provisto es válido por 10 minutos. </h4>"
                };
            Mensaje.Body = HtmlMensaje;
            SmtpClient ClienteSMTP = new SmtpClient ();
            //Pendiente: Ver si la ULP tiene un servidor SMTP (o IMAP, o POP3), y usar éso en lugar de un proveedor externo.
            ClienteSMTP.Connect ("", 25, false);
            ClienteSMTP.Authenticate (Config["Correo:UsuarioSMTP"], Config["Correo:ClaveSMTP"]);
            await ClienteSMTP.SendAsync (Mensaje);

            await Contexto.SaveChangesAsync();
            return Ok("Revisa tu correo, allí recibirás tu nueva contraseña.");
        }
        catch (Exception ex) {
            return StatusCode(500, ex);
        }
    }
    
    /*
    [Obsolete("Reemplazado por /Api/Auth/Login")]
    [AllowAnonymous]
    [HttpPost("Login")]
    [SwaggerOperation(
        Summary = "Inicia la sesión del Usuario.",
        Description = "Verifica documento y clave, y retorna 200 si es exitoso, junto con un JWT y los datos públicos del Usuario."
    )]
    [SwaggerResponse(200, "Login exitoso.")]
    [SwaggerResponse(400, "Documento o clave incorrectos.")]
    public async Task<IActionResult> IniciarSesiónUsuario([FromForm] LoginViewUsuario login)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos obligatorios.");

        // Buscar por documento
        var Usuario = await Contexto.Usuarios.FirstOrDefaultAsync(i =>
            i.NumDocumento == Int32.Parse(login.Identificador) // Ahora el LoginView recibe un string, no un int. Si puede convertirse a int, sigue.
        );

        if (Usuario == null)
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

        if (claveHash != Usuario.Clave)
            return BadRequest("Clave incorrecta.");

        // Claims públicos (incluye DNI, no clave)
        var claims = new[]
        {
            new Claim("NumDocumento", Usuario.NumDocumento.ToString()),
            new Claim("TipoDocumento", Usuario.TipoDocumento ?? ""),
            new Claim("Correo", Usuario.Correo ?? ""),
            new Claim("Teléfono", Usuario.Teléfono ?? ""),
            new Claim("Asociación", Usuario.Asociación ?? ""),
            new Claim("Nombre", Usuario.Nombre ?? ""),
            new Claim(ClaimTypes.Role, "Usuario")
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
                    NumDocumento = Usuario.NumDocumento,
                    TipoDocumento = Usuario.TipoDocumento,
                    Correo = Usuario.Correo,
                    Teléfono = Usuario.Teléfono,
                    Asociación = Usuario.Asociación,
                    Nombre = Usuario.Nombre,
                },
            }
        );
    } 
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginViewUsuario login)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos.");

        var claveHasheada = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: login.Clave,
                salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 4096,
                numBytesRequested: 256 / 8
            )
        );

        var Usuario = await Contexto.Usuarios.FirstOrDefaultAsync(i =>
            i.NumDocumento == login.NumDocumento
        );

        if (Usuario == null || Usuario.Clave != claveHasheada)
        {
            return BadRequest("DNI o clave incorrectos.");
        }

        // Guardamos nombre y DNI en la sesión (si está habilitada)
        HttpContext.Session.SetString("Nombre", Usuario.Nombre);
        HttpContext.Session.SetInt32("DNI", Usuario.NumDocumento);

        return Ok("Inicio de sesión exitoso.");
    }
    */
}
