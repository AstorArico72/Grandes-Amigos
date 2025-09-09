// Controllers/AdminApiController.cs
//
// Controlador de API para endpoints de administración.
// ⚠️ Importante: Este controlador NO debe devolver Views ni hacer RedirectToAction.
//                 Devuelve JSON (Ok/BadRequest/etc.).
//
// Rutas base: /Api/Admin/*
//
// Requisitos:
// - Política "Ministerio" configurada en Program.cs para proteger endpoints sensibles.
// - IConfiguration con Salt y JWT keys.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api
{
    [ApiController]
    [ApiVersionNeutral]
    [Route("/Api/Admin")]
    public class AdminApiController : Controller
    {
        private readonly ContextoDb _ctx;
        private readonly IConfiguration _cfg;
        private readonly ILogger<AdminApiController> _logger;

        public AdminApiController(
            ContextoDb ctx,
            IConfiguration cfg,
            ILogger<AdminApiController> logger
        )
        {
            _ctx = ctx;
            _cfg = cfg;
            _logger = logger;
        }

        // -----------------------------
        // POST /Api/Admin/Nuevo
        // Crea un administrador nuevo
        // -----------------------------
        [AllowAnonymous]
        [HttpPost("Nuevo")]
        [SwaggerOperation(
            Summary = "Crea un administrador",
            Description = "Registra un nuevo admin con clave hasheada."
        )]
        [SwaggerResponse(201, "Administrador creado")]
        [SwaggerResponse(400, "Datos inválidos")]
        public async Task<IActionResult> RegistrarAdmin([FromForm] Administrador nuevo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Faltan datos requeridos.");

            var salt = _cfg["Salt"];
            if (string.IsNullOrEmpty(salt))
                return StatusCode(500, "Falta configurar 'Salt' en appsettings.");

            // Hash de la clave
            nuevo.Clave = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: nuevo.Clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );

            await _ctx.Admins.AddAsync(nuevo);
            await _ctx.SaveChangesAsync();
            return StatusCode(201);
        }

        // -----------------------------
        // POST /Api/Admin/Login
        // Autentica y devuelve JWT
        // -----------------------------
        [AllowAnonymous]
        [HttpPost("Login")]
        [SwaggerOperation(
            Summary = "Login admin",
            Description = "Valida credenciales y devuelve JWT."
        )]
        [SwaggerResponse(200, "Login OK")]
        [SwaggerResponse(400, "Usuario o clave inválidos")]
        public async Task<IActionResult> Login([FromForm] LoginViewAdmin login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Faltan datos obligatorios.");

            var admin = await _ctx.Admins.FirstOrDefaultAsync(a =>
                a.NombreUsuario == login.NombreUsuario
            );
            if (admin is null)
                return BadRequest("Usuario no registrado.");

            var salt = _cfg["Salt"];
            if (string.IsNullOrEmpty(salt))
                return StatusCode(500, "Falta configurar 'Salt' en appsettings.");

            var claveHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );

            if (claveHash != admin.Clave)
                return BadRequest("Clave incorrecta.");

            // Claims (rol y ministerio)
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, admin.NombreUsuario),
                new Claim(ClaimTypes.Role, "Ministerio"),
                new Claim("IdMinisterio", admin.IdMinisterio.ToString()),
            };

            var jwtKey = _cfg["TokenAuthentication:SecretKey"] ?? _cfg["JwtKey"];
            if (string.IsNullOrEmpty(jwtKey))
                return StatusCode(
                    500,
                    "Falta configurar 'TokenAuthentication:SecretKey' o 'JwtKey'."
                );

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _cfg["TokenAuthentication:Issuer"] ?? "GrandesAmigos",
                audience: _cfg["TokenAuthentication:Audience"] ?? "GrandesAmigos",
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // ✅ API devuelve JSON (NO redirect)
            return Ok(
                new { token = tokenString, admin = new { admin.NombreUsuario, admin.IdMinisterio } }
            );
        }

        // -----------------------------
        // GET /Api/Admin/Kpis
        // KPI simples para el dashboard (opcional)
        // -----------------------------
        [Authorize(Policy = "Ministerio")]
        [HttpGet("Kpis")]
        [SwaggerOperation(
            Summary = "KPIs de tablero",
            Description = "Totales y últimos eventos para el panel."
        )]
        public async Task<IActionResult> Kpis()
        {
            var totalUsuarios = await _ctx.Usuarios.CountAsync();
            var totalEventos = await _ctx.Eventos.CountAsync();
            var ultimosEventos = await _ctx
                .Eventos.OrderByDescending(e => e.Fecha)
                .Take(5)
                .Select(e => new
                {
                    e.ID,
                    e.Título,
                    e.Fecha,
                })
                .ToListAsync();

            return Ok(
                new
                {
                    totalUsuarios,
                    totalEventos,
                    ultimosEventos,
                }
            );
        }
    }
}
