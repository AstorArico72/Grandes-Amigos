using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Grandes_Amigos.Api
{
    [ApiController]
    [ApiVersionNeutral]
    [Route("/Api/Auth")]
    public class AuthController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IConfiguration Config;
        private ContextoDb Contexto;

        public AuthController(ILogger<UsuariosController> logger, ContextoDb contexto, IConfiguration ajustes)
        {
            _logger = logger;
            Contexto = contexto;
            Config = ajustes;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> IniciarSesion([FromForm] LoginViewUsuario login)
        {
            Usuario? UsuarioEncontrado = null;
            Administrador? AdminEncontrado = null;

            if (!ModelState.IsValid)
            {
                return BadRequest("Faltan datos obligatorios.");
            }

            try
            {
                int? dni = null;
                if (login.Identificador != null)
                {
                    dni = int.Parse(login.Identificador);
                    if (dni != null)
                    {
                        UsuarioEncontrado = await Contexto.Usuarios.FindAsync(dni);
                    }
                }
            }
            catch (FormatException)
            {
                AdminEncontrado = await Contexto.Admins.FirstOrDefaultAsync(admin => admin.NombreUsuario == login.Identificador);
            }

            var saltString = Config["Salt"];
            if (string.IsNullOrEmpty(saltString))
            {
                return StatusCode(500, "No se configuro el salt para el hash de contrasenas.");
            }

            string? claveHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(saltString),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );

            if ((UsuarioEncontrado != null && claveHash != UsuarioEncontrado.Clave) || (AdminEncontrado != null && claveHash != AdminEncontrado.Clave))
            {
                return BadRequest("Clave incorrecta.");
            }

            var jwtKey = Config["TokenAuthentication:SecretKey"] ?? Config["JwtKey"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, "No se configuro la clave JWT.");
            }

            var claims = new List<Claim>();

            if (UsuarioEncontrado != null && AdminEncontrado == null)
            {
                claims.Add(new Claim("NumDocumento", UsuarioEncontrado.NumDocumento.ToString()));
                claims.Add(new Claim("TipoDocumento", UsuarioEncontrado.TipoDocumento ?? ""));
                claims.Add(new Claim("Correo", UsuarioEncontrado.Correo ?? ""));
                claims.Add(new Claim("Nombre", UsuarioEncontrado.Nombre ?? ""));
                claims.Add(new Claim(ClaimTypes.Role, "Usuario"));
            }
            else if (UsuarioEncontrado == null && AdminEncontrado != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, AdminEncontrado.NombreUsuario));
                claims.Add(new Claim(ClaimTypes.Role, "Ministerio"));
                claims.Add(new Claim("IdMinisterio", AdminEncontrado.IdMinisterio.ToString()));
                claims.Add(new Claim("AdminID", AdminEncontrado.ID.ToString()));
                claims.Add(new Claim("AdminNombre", AdminEncontrado.NombreUsuario));
                claims.Add(new Claim("AdminMinisterio", AdminEncontrado.IdMinisterio.ToString()));
            }

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

            if (UsuarioEncontrado != null && AdminEncontrado == null)
            {
                var RespuestaUsuario = new
                {
                    token = tokenString,
                usuario = new
                {
                    NumDocumento = UsuarioEncontrado.NumDocumento,
                    TipoDocumento = UsuarioEncontrado.TipoDocumento,
                    Correo = UsuarioEncontrado.Correo,
                    Nombre = UsuarioEncontrado.Nombre,
                }
            };
                return Ok(RespuestaUsuario);
            }
            else if (UsuarioEncontrado == null && AdminEncontrado != null)
            {
                await HttpContext.SignInAsync(
                    "AdminCookie",
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "AdminCookie")),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(12)
                    }
                );

                var RespuestaAdmin = new
                {
                    token = tokenString,
                    admin = new
                    {
                        NombreUsuario = AdminEncontrado.NombreUsuario,
                        IdMinisterio = AdminEncontrado.IdMinisterio
                    }
                };
                return Ok(RespuestaAdmin);
            }
            else
            {
                return BadRequest("Nombre incorrecto.");
            }
        }
    }
}

