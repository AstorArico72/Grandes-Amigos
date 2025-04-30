using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
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
    public async Task<IActionResult> IniciarSesión ([FromForm] LoginView LoginData) {
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
            return Ok (new JwtSecurityTokenHandler ().WriteToken (Token));
        }
    }
}