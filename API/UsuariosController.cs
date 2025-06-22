using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Usuarios")]
public class UsuariosController : Controller {
    private ContextoDb Contexto;
    private readonly IConfiguration Config;

    public UsuariosController (ContextoDb contexto, IConfiguration ajustes) {
        Contexto = contexto;
        Config = ajustes;
    }

    [AllowAnonymous]
    [HttpPost("Nuevo")]
    [SwaggerOperation (
        Summary = "Crea un nuevo usuario.",
        Description = "Crea una nueva entrada en la tabla `Usuarios`. La contraseña no es guardada de forma textual, sino que se convierte a un `hash`."
    )]
    [SwaggerResponse (201, "El usuario fue creado con éxito.")]
    [SwaggerResponse (400, "Un campo está vacío o es inválido.")]
    [SwaggerResponse (500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public async Task<IActionResult> NuevoUsuario ([FromForm] Usuario NuevoUsuario) {
        try {
            if (ModelState.IsValid) {
                NuevoUsuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			        password: NuevoUsuario.Clave,
			        salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
			        prf: KeyDerivationPrf.HMACSHA256,
			        iterationCount: 4096,
			        numBytesRequested: 256 / 8
                ));
                await Contexto.Usuarios.AddAsync (NuevoUsuario);
                await Contexto.SaveChangesAsync ();
                return Created ();
            } else {
                return BadRequest ("Un campo es inválido.");
            }
        } catch (MySqlException ex) {
            return StatusCode (500, ex);
        }
    }

    [AllowAnonymous]
    [HttpPost("Ingresar")]
    [SwaggerOperation (
        Summary = "Inicia sesión.",
        Description = "Si el nombre de usuario es válido y la contraseña corresponde al usuario con ése nombre, genera un `JsonWebToken` que será utilizado para acceder a ciertos endpoints protegidos."
        )]
    [SwaggerResponse (200, "Se ha iniciado la sesión.")]
    [SwaggerResponse (400, "El nombre de usuario o la clave es incorrecto.")]
    public async Task<IActionResult> IniciarSesiónAdmin ([FromForm] LoginView LoginData) {
        Usuario? UsuarioSeleccionado = await Contexto.Usuarios.FirstOrDefaultAsync (usuario => usuario.NombreUsuario == LoginData.NombreUsuario);
        string ContraseñaConHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: LoginData.Clave,
			salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 4096,
			numBytesRequested: 256 / 8
        ));
        var Llave = new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes(Config["TokenAuthentication:SecretKey"]));
        var Credenciales = new SigningCredentials (Llave, SecurityAlgorithms.HmacSha256);
        if (UsuarioSeleccionado == null || ContraseñaConHash != UsuarioSeleccionado.Clave) {
            return BadRequest ("Usuario o clave incorrectos.");
        } else {
            Claim ClaimNombre = new Claim (ClaimTypes.Name, UsuarioSeleccionado.NombreUsuario);
            Claim ClaimMinisterio = new Claim ("IdMinisterio", UsuarioSeleccionado.IdMinisterio.ToString ());
            Claim ClaimIdUsuario = new Claim ("IdUsuario", UsuarioSeleccionado.ID.ToString ());
            Claim ClaimRol = new Claim (ClaimTypes.Role, "Ministerio");
            List<Claim> ClaimList = new List<Claim> ([ClaimNombre, ClaimMinisterio, ClaimIdUsuario, ClaimRol]);

            var Token = new JwtSecurityToken (
                issuer: Config["TokenAuthentication:Issuer"],
                audience: Config["TokenAuthentication:Audience"],
                claims: ClaimList,
                expires: DateTime.Now.AddHours (24),
                signingCredentials: Credenciales
            );

            Request.Headers.Authorization = new JwtSecurityTokenHandler().WriteToken(Token);

            return RedirectToAction("Dashboard", "Admin");
        }
    }
}