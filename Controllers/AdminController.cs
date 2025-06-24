using Microsoft.AspNetCore.Mvc;

namespace Grandes_Amigos.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        [HttpGet("Login")] // 👈 este permite mostrar el formulario
        public IActionResult Login()
        {
            return View();
        }

        [Obsolete("Reemplazado por /Api/Usuarios/Login, pero guardado por si acaso.")]
        [HttpPost("Login")] // 👈 este procesa el formulario
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "1234")
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Credenciales inválidas";
            return View();
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            // 🔓 Acceso libre mientras trabajo en el front
            return View();
        }
    }
}
