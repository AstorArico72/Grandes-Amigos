using System.Security.Cryptography;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Admin")]
public class InscritosController : Controller
{
    private readonly ContextoDb Contexto;
    private readonly IConfiguration Config;

    public InscritosController(ContextoDb contexto, IConfiguration config)
    {
        Contexto = contexto;
        Config = config;
    }

    [AllowAnonymous]
    [HttpPost("Nuevo")]
    public async Task<IActionResult> RegistrarAdmin([FromForm] Administrador nuevo)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos requeridos.");

        try
        {
            // Hashear clave
            nuevo.Clave = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: nuevo.Clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );

            await Contexto.Admins.AddAsync(nuevo);
            await Contexto.SaveChangesAsync();

            return Created();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Error al guardar el administrador: {ex.Message}");
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [SwaggerOperation(
        Summary = "Inicia la sesión del admin.",
        Description = "Verifica nombre y clave, y retorna 200 si es exitoso, junto con un JWT y los datos públicos del admin."
    )]
    [SwaggerResponse(200, "Login exitoso.")]
    [SwaggerResponse(400, "Nombre o clave incorrectos.")]
    public async Task<IActionResult> IniciarSesiónAdmin([FromForm] LoginViewAdmin login)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos obligatorios.");

        // Buscar por nombre
        var Usuario = await Contexto.Admins.FirstOrDefaultAsync(i =>
            i.NombreUsuario == login.NombreUsuario
        );

        if (Usuario == null)
            return BadRequest("Usuario no registrado.");

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

        // Claims públicos
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, Usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, "Ministerio"),
            new Claim("IdMinisterio", Usuario.IdMinisterio.ToString())
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
                admin = new
                {
                    ID = Usuario.ID,
                    Nombre = Usuario.NombreUsuario,
                    IdMinisterio = Usuario.IdMinisterio
                },
            }
        );
    }

}
