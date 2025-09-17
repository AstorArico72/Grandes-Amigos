// Controllers/AdminApiController.cs
//
// Controlador de API para endpoints de administración.
// ⚠️ NO devuelve Views ni hace RedirectToAction: responde JSON.
// Rutas base: /Api/Admin/*
// - POST /Api/Admin/Nuevo   → alta de administrador (hash de clave)
// - POST /Api/Admin/Login   → valida credenciales y devuelve { token, admin }
// - GET  /Api/Admin/Kpis    → KPIs para el dashboard (protegido)
//
// Requisitos:
// - Política "Ministerio" en Program.cs para proteger endpoints sensibles.
// - IConfiguration con Salt y claves JWT.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Grandes_Amigos.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api
{
    [ApiController]
    [ApiVersionNeutral]
    [Route("/Api/Admin")]
    [Produces("application/json")]
    public class AdminApiController : Controller
    {
        private readonly ContextoDb _ctx;
        private readonly IConfiguration _cfg;
        private readonly ILogger<AdminApiController> _logger;
        private readonly EmailService _emailService;

        public AdminApiController(
            ContextoDb ctx,
            IConfiguration cfg,
            EmailService emailService,
            ILogger<AdminApiController> logger
        )
        {
            _ctx = ctx;
            _cfg = cfg;
            _logger = logger;
            _emailService = emailService;
        }

        // --------------------------------------------------------------------
        // POST /Api/Admin/Nuevo
        // Crea un administrador nuevo (tabla administradores).
        // --------------------------------------------------------------------
        [AllowAnonymous]
        [HttpPost("Nuevo")]
        [SwaggerOperation(
            Summary = "Crea un administrador",
            Description = "Registra un nuevo admin con clave hasheada (PBKDF2)."
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

            // Hash PBKDF2 (HMACSHA256, 4096 iteraciones, 256 bits)
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

        // --------------------------------------------------------------------
        // POST /Api/Admin/Login
        // Autentica y devuelve JWT + datos públicos del admin.
        // (Compatible con admin-login.js vía fetch: responde JSON)
        // --------------------------------------------------------------------
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

            // Clave JWT
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

            // ✅ API responde JSON (para fetch de admin-login.js)
            return Ok(
                new { token = tokenString, admin = new { admin.NombreUsuario, admin.IdMinisterio } }
            );
        }

        // --------------------------------------------------------------------
        // GET /Api/Admin/Kpis
        // KPI simples para el dashboard (protegido por la policy "Ministerio")
        // --------------------------------------------------------------------
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

        // Copia de /Api/Usuarios/ClaveOlvidada, adaptada para usar EmailService.
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
                Administrador? UsuarioSeleccionado = await _ctx.Admins.FirstOrDefaultAsync(item =>
                    item.Email == correo
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
                        salt: System.Text.Encoding.UTF8.GetBytes(_cfg["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 100,
                        numBytesRequested: 64
                    )
                );

                // Guardar token en la base (para admin)
                Recuperación NuevoToken = new Recuperación
                {
                    Token_Recuperación = TokenHash,
                    ID_Admin = UsuarioSeleccionado.ID, // <- asigno a la columna ID_Admin
                    Válido_Hasta = DateTime.Now.AddMinutes(10),
                    Rol = "admin",
                };

                _ctx.Tokens.Add(NuevoToken);
                await _ctx.SaveChangesAsync();

                // Construir enlace de recuperación
                string link =
                    $"http://127.0.0.1:5020/Api/Admin/RecuperarCuenta?TokenRecuperacion={Uri.EscapeDataString(TokenSinHash)}";

                string body =
                    $@"
            <h1>Saludos</h1>
            <p>{UsuarioSeleccionado.NombreUsuario}, recibiste este correo porque hubo una solicitud para recuperar tu clave. 
            Si fuiste vos, <a href='{link}'>haz clic aquí</a> para cambiar tu contraseña.</p>
            <h4>El enlace es válido por 10 minutos.</h4>";

                // ✅ Usar el EmailService
                await _emailService.SendEmailAsync(
                    UsuarioSeleccionado.Email,
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
        public IActionResult RecuperarCuenta([FromQuery(Name = "TokenRecuperacion")] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token vacío.");

            // recalcular hash, igual que en ClaveOlvidada
            string TokenHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: token,
                    salt: System.Text.Encoding.UTF8.GetBytes(_cfg["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 100,
                    numBytesRequested: 64
                )
            );

            var tokenRow = _ctx.Tokens.Find(TokenHash);
            if (tokenRow == null || DateTime.Now >= tokenRow.Válido_Hasta)
                return BadRequest("Token inválido o expirado.");

            // redirigir al formulario estático con el token
            string url = $"/recuperar.html?TokenRecuperacion={Uri.EscapeDataString(token)}";
            return Redirect(url);
        }

        // DTO para recibir el POST
        public class ChangePasswordDto
        {
            public string TokenRecuperacion { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("CambiarClave")]
        [SwaggerOperation(Summary = "Cambia la contraseña usando el token de recuperación.")]
        public async Task<IActionResult> CambiarClave([FromBody] ChangePasswordDto dto)
        {
            if (dto == null)
                return BadRequest("Payload inválido.");
            if (string.IsNullOrWhiteSpace(dto.TokenRecuperacion))
                return BadRequest("Token vacío.");
            if (
                string.IsNullOrWhiteSpace(dto.Password)
                || string.IsNullOrWhiteSpace(dto.ConfirmPassword)
            )
                return BadRequest("Las contraseñas son requeridas.");
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Las contraseñas no coinciden.");

            // Opcional: validar política de contraseña
            if (dto.Password.Length < 8)
                return BadRequest("La contraseña debe tener al menos 8 caracteres.");

            try
            {
                // Recalcular hash del token (igual que al crear el token)
                string tokenHash = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: dto.TokenRecuperacion,
                        salt: System.Text.Encoding.UTF8.GetBytes(_cfg["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 100,
                        numBytesRequested: 64
                    )
                );

                Recuperación tokenRow = await _ctx.Tokens.FindAsync(tokenHash);
                if (tokenRow == null)
                    return BadRequest("Token inválido.");

                if (DateTime.Now >= tokenRow.Válido_Hasta)
                {
                    return BadRequest("El token expiró.");
                }

                // Determinar a qué entidad corresponde (admin o usuario)
                if (tokenRow.ID_Admin.HasValue)
                {
                    var admin = await _ctx.Admins.FindAsync(tokenRow.ID_Admin.Value);
                    if (admin == null)
                        return BadRequest("Administrador no encontrado.");

                    string newPwdHash = Convert.ToBase64String(
                        KeyDerivation.Pbkdf2(
                            password: dto.Password,
                            salt: System.Text.Encoding.UTF8.GetBytes(_cfg["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA256, // <- SHA256 (coincide con Registrar)
                            iterationCount: 4096, // <- misma cantidad de iteraciones
                            numBytesRequested: 256 / 8 // <- 256 bits / 8 = 32 bytes
                        )
                    );

                    admin.Clave = newPwdHash;

                    // Eliminar el token (o marcar inválido)
                    _ctx.Tokens.Remove(tokenRow);

                    await _ctx.SaveChangesAsync();
                    return Ok("Contraseña cambiada correctamente para el administrador.");
                }
                else if (tokenRow.ID_Usuario.HasValue)
                {
                    var usuario = await _ctx.Usuarios.FindAsync(tokenRow.ID_Usuario.Value);
                    if (usuario == null)
                        return BadRequest("Usuario no encontrado.");

                    string newPwdHash = Convert.ToBase64String(
                        KeyDerivation.Pbkdf2(
                            password: dto.Password,
                            salt: System.Text.Encoding.UTF8.GetBytes(_cfg["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA256, // <- SHA256 (coincide con Registrar)
                            iterationCount: 4096, // <- misma cantidad de iteraciones
                            numBytesRequested: 256 / 8 // <- 256 bits / 8 = 32 bytes
                        )
                    );

                    usuario.Clave = newPwdHash;

                    _ctx.Tokens.Remove(tokenRow);

                    await _ctx.SaveChangesAsync();
                    return Ok("Contraseña cambiada correctamente.");
                }
                else
                {
                    return BadRequest("Token no asociado a ningún usuario.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CambiarClave");
                return StatusCode(500, "Error interno.");
            }
        }
    }
}
