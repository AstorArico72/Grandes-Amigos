using Microsoft.AspNetCore.Mvc;

namespace Grandes_Amigos.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(); // libre para que el usuario pueda ingresar sus credenciales hasta que este listo el panel
        }

        [HttpPost("Login")]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            // 🔓 Acceso libre mientras trabajo en el front
            return View();
        }
    }
}
