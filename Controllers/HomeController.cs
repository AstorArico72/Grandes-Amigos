using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;

namespace Grandes_Amigos.Controllers;

[Route("Home")]
public class HomeController : Controller {
    // HomeController.cs
    // 
    // Página de inicio ("landing").
    // Patrón: [host]/Home/
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) {
        _logger = logger;
    }

    [HttpGet("/")]
    public IActionResult Index() {
        return View();
    }

    [HttpGet("Privacy")]
    public IActionResult Privacy() {
        return View();
    }

    //Pendiente: Manejo de errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
