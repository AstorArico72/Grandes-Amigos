using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;

namespace Grandes_Amigos.Controllers;

[AllowAnonymous]
[Route("Eventos")]
public class EventosController : Controller {
    // EventosController.cs
    // 
    // CRUD de eventos
    // Patrón: [host]/Eventos
    private readonly ILogger<EventosController> _logger;

    public EventosController(ILogger<EventosController> logger) {
        _logger = logger;
    }

    [HttpGet("/")]
    public IActionResult Index() {
        return View();
    }

    [HttpGet("{id}")]
    public IActionResult VerEvento(int id) {
        //Éste método carga la vista de detalles.
        //Pendiente: Cargar vistas Angular desde Dotnet.
        //Es importante que las vistas de éste endpoint tengan un botón que enlace a /Inscripcion, con el ID del evento cargado en el querystring.
        //Es decir, /Inscripcion?idEvento={id}
        //Ve a InscripcionController.cs para saber por qué.
        return View();
    }

    [Authorize] //Pendiente: Crear políticas y roles
    [HttpGet("/Crear")]
    public IActionResult CrearEvento () {
        //Éste método carga el formulario para agregar eventos.
        return View ();
    }

    [Authorize]
    [HttpPost("/Nuevo")]
    public IActionResult NuevoEvento () {
        //Pendiente: Objeto modelo para la entidad Evento.
        //Pendiente: Lógica para agregar eventos.
        return Created ();
    }

    //Pendiente: Manejo de errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
