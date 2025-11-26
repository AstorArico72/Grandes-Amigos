// Controllers/AdminController.cs
//
// MVC del panel (solo renderiza vistas Razor).
// SIN [Authorize] porque usamos JWT-only y los datos del panel
// se obtienen desde JS llamando a /Api/* con Bearer.

using System;
using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ContextoDb _ctx;
        private readonly IConfiguration _cfg;

        public AdminController(ContextoDb ctx, IConfiguration cfg)
        {
            _ctx = ctx;
            _cfg = cfg;
        }

        private bool EsSuperAdmin()
        {
            var ministerio = User?.FindFirst("AdminMinisterio")?.Value
                ?? User?.FindFirst("IdMinisterio")?.Value;
            return ministerio == "1";
        }

        private string HashPassword(string clave)
        {
            var salt = _cfg["Salt"];
            if (string.IsNullOrEmpty(salt))
                throw new InvalidOperationException("Falta configurar 'Salt' para hashear contrasenas.");

            return Convert.ToBase64String(
                Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
                    password: clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(salt),
                    prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );
        }

        // GET /Admin/Login
        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("~/Views/Admin/Login.cshtml");
        }

        // GET /Admin/NuevoEvento
        [HttpGet("NuevoEvento")]
        public IActionResult Nuevo()
        {
            return View("~/Views/Admin/NuevoEvento.cshtml");
        }

        // /Admin redirige al Dashboard
        [HttpGet("")]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // GET /Admin/Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var vm = new DashboardViewModel
            {
                TotalUsuarios = await _ctx.Usuarios.CountAsync(),
                TotalEventos = await _ctx.Eventos.CountAsync(),
                EventosRecientes = await _ctx
                    .Eventos.OrderByDescending(e => e.Fecha)
                    .Take(5)
                    .ToListAsync(),
            };
            return View("~/Views/Admin/Dashboard.cshtml", vm);
        }

        // GET /Admin/Usuarios
        [HttpGet("Usuarios")]
        public IActionResult Usuarios() => View("~/Views/Admin/Usuarios.cshtml");

        // GET /Admin/Administradores
        [HttpGet("Administradores")]
        public async Task<IActionResult> Administradores()
        {
            if (!EsSuperAdmin())
                return Forbid();

            var admins = await _ctx
                .Admins.Join(
                    _ctx.Ministerios,
                    a => a.IdMinisterio,
                    m => m.ID,
                    (a, m) =>
                        new AdminListItemViewModel
                        {
                            ID = a.ID,
                            NombreUsuario = a.NombreUsuario,
                            Email = a.Email,
                            IdMinisterio = a.IdMinisterio,
                            MinisterioNombre = m.Nombre,
                        }
                )
                .OrderBy(a => a.NombreUsuario)
                .ToListAsync();

            return View("~/Views/Admin/Administradores.cshtml", admins);
        }

        // GET /Admin/NuevoAdministrador
        [HttpGet("NuevoAdministrador")]
        public async Task<IActionResult> NuevoAdministrador()
        {
            if (!EsSuperAdmin())
                return Forbid();

            var ministerios = await _ctx.Ministerios.OrderBy(m => m.Nombre).ToListAsync();
            var vm = new AdminFormViewModel
            {
                IdMinisterio = 1,
                Ministerios = ministerios,
                EsEdicion = false,
            };
            return View("~/Views/Admin/NuevoAdministrador.cshtml", vm);
        }

        // POST /Admin/NuevoAdministrador
        [HttpPost("NuevoAdministrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoAdministrador(AdminFormViewModel model)
        {
            if (!EsSuperAdmin())
                return Forbid();

            model.Ministerios = await _ctx.Ministerios.OrderBy(m => m.Nombre).ToListAsync();

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Clave))
            {
                if (string.IsNullOrWhiteSpace(model.Clave))
                    ModelState.AddModelError(nameof(model.Clave), "La contrasena es requerida.");
                return View("~/Views/Admin/NuevoAdministrador.cshtml", model);
            }

            var existe = await _ctx.Admins.AnyAsync(a =>
                a.NombreUsuario == model.NombreUsuario || a.Email == model.Email
            );
            if (existe)
            {
                ModelState.AddModelError(string.Empty, "Ya existe un administrador con ese usuario o correo.");
                return View("~/Views/Admin/NuevoAdministrador.cshtml", model);
            }

            var admin = new Administrador
            {
                NombreUsuario = model.NombreUsuario.Trim(),
                Email = model.Email.Trim(),
                IdMinisterio = model.IdMinisterio,
                Clave = HashPassword(model.Clave!),
            };

            _ctx.Admins.Add(admin);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Administradores));
        }

        // GET /Admin/EditarAdministrador/5
        [HttpGet("EditarAdministrador/{id:int}")]
        public async Task<IActionResult> EditarAdministrador(int id)
        {
            if (!EsSuperAdmin())
                return Forbid();

            var admin = await _ctx.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            var vm = new AdminFormViewModel
            {
                ID = admin.ID,
                NombreUsuario = admin.NombreUsuario,
                Email = admin.Email,
                IdMinisterio = admin.IdMinisterio,
                Ministerios = await _ctx.Ministerios.OrderBy(m => m.Nombre).ToListAsync(),
                EsEdicion = true,
            };
            return View("~/Views/Admin/NuevoAdministrador.cshtml", vm);
        }

        // POST /Admin/EditarAdministrador/5
        [HttpPost("EditarAdministrador/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAdministrador(int id, AdminFormViewModel model)
        {
            if (!EsSuperAdmin())
                return Forbid();

            if (id != model.ID)
                return BadRequest();

            var admin = await _ctx.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            model.Ministerios = await _ctx.Ministerios.OrderBy(m => m.Nombre).ToListAsync();
            model.EsEdicion = true;

            if (!ModelState.IsValid)
                return View("~/Views/Admin/NuevoAdministrador.cshtml", model);

            var existe = await _ctx.Admins.AnyAsync(a =>
                (a.NombreUsuario == model.NombreUsuario || a.Email == model.Email) && a.ID != id
            );
            if (existe)
            {
                ModelState.AddModelError(string.Empty, "Ya existe otro administrador con ese usuario o correo.");
                return View("~/Views/Admin/NuevoAdministrador.cshtml", model);
            }

            admin.NombreUsuario = model.NombreUsuario.Trim();
            admin.Email = model.Email.Trim();
            admin.IdMinisterio = model.IdMinisterio;

            if (!string.IsNullOrWhiteSpace(model.Clave))
            {
                if (model.Clave != model.ConfirmarClave)
                {
                    ModelState.AddModelError(nameof(model.ConfirmarClave), "Las contrasenas no coinciden.");
                    return View("~/Views/Admin/NuevoAdministrador.cshtml", model);
                }

                admin.Clave = HashPassword(model.Clave);
            }

            _ctx.Admins.Update(admin);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Administradores));
        }

        // POST /Admin/EliminarAdministrador/5
        [HttpPost("EliminarAdministrador/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAdministrador(int id)
        {
            if (!EsSuperAdmin())
                return Forbid();

            var admin = await _ctx.Admins.FindAsync(id);
            if (admin == null)
                return NotFound();

            _ctx.Admins.Remove(admin);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Administradores));
        }

        // POST /Admin/EstablecerCookie
        [HttpPost("EstablecerCookie")]
        [AllowAnonymous]
        public async Task<IActionResult> EstablecerCookie([FromForm] string nombre, [FromForm] string ministerio, [FromForm] string id)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nombre ?? string.Empty),
                new Claim(ClaimTypes.Role, "Ministerio"),
                new Claim("AdminID", id ?? string.Empty),
                new Claim("AdminMinisterio", ministerio ?? string.Empty),
            };

            var identity = new ClaimsIdentity(claims, "AdminCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "AdminCookie",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                }
            );

            return Ok();
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login", "Admin");
        }
    }
}
