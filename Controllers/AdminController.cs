// Controllers/AdminController.cs
//
// MVC del panel (sólo renderiza vistas Razor).
// SIN [Authorize] porque usamos JWT-only y los datos del panel
// se obtienen desde JS llamando a /Api/* con Bearer.

using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ContextoDb _ctx;

        public AdminController(ContextoDb ctx)
        {
            _ctx = ctx;
        }

        // GET /Admin/Login
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View("~/Views/Admin/Login.cshtml");
        }

        // /Admin → redirige al Dashboard
        [HttpGet("")]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // GET /Admin/Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            // Si no querés cargar nada desde servidor, podés
            // pasar null y que JS llene todo con la API.
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
    }
}
