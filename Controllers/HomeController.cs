using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var hero = new ConfigHero
        {
            ImagenUrl = "/img/hero1.png", // Ajustá la ruta real
            Titulo = "Te damos la bienvenida",
            Subtitulo = "Plataforma para mayores",
            TextoBoton = "Ingresar",
        };

        return View(hero);
    }
}
