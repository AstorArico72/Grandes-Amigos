using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Admin")]
public class AdminController : Controller
{
    private readonly ContextoDb Contexto;
    private readonly IConfiguration Config;

    public AdminController(ContextoDb contexto, IConfiguration config)
    {
        Contexto = contexto;
        Config = config;
    }

    [HttpGet("Dashboard")] // o la ruta que corresponda si la tienes definida
    public async Task<IActionResult> Dashboard()
    {
        // 1. Creamos la "caja" para nuestros datos.
        var viewModel = new DashboardViewModel();

        // 2. Llenamos la caja con datos de la base de datos.
        viewModel.TotalUsuarios = await Contexto.Usuarios.CountAsync();
        viewModel.TotalEventos = await Contexto.Eventos.CountAsync();
        viewModel.EventosRecientes = await Contexto
            .Eventos.OrderByDescending(e => e.Fecha)
            .Take(5) // Tomamos los 5 más recientes
            .ToListAsync();

        // 3. Enviamos la caja llena de datos a la vista.
        return View(viewModel);
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
            new Claim("IdMinisterio", Usuario.IdMinisterio.ToString()),
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

        // Redirige al panel
        return RedirectToAction("Dashboard");
    }
}
