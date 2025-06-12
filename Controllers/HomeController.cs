using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Public/Index.cshtml"); // ✅ esto sí lo encuentra
    }
}
