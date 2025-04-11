using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;

namespace Grandes_Amigos.Controllers;

[AllowAnonymous] //El uso de [AllowAnonymous] es porque éste endpoint es usado por los visitantes del sitio, quienes, como se ha discutido antes, no necesitan registrarse.
[Route("Inscripcion")]
public class InscripcionController : Controller {
    // InscripcionController.cs
    // 
    // CRUD de inscripciones
    // Patrón: [host]/Inscripcion
    private readonly ILogger<InscripcionController> _logger;

    public InscripcionController(ILogger<InscripcionController> logger) {
        _logger = logger;
    }

    [HttpGet("/")]
    public IActionResult FormularioInscripcion([FromQuery] int IdEvento) {
        //Éste método carga el formulario para la inscripción.
        //Cada evento en /Eventos/{id} tendrá un botón que enlaza a ésta URL, con el ID del evento en el querystring.
        //En la vista, tiene que haber un <input> "escondido" (type="hidden") que tiene el nombre "IdEvento".
        //Ése campo es llenado por el valor del querystring.
        //El querystring es importante, ver explicación debajo.
        return View();
    }

    [HttpPost("/Nueva")]
    public IActionResult NuevaInscripcion () {
        //Pendiente: Objeto modelo para la entidad inscripción.
        //Pendiente: Implementar lógica de inscripción.

        //El campo que hace referencia al evento es llenado por el querystring en "FormularioInscripcion".
        //Es decir, si uno se ha inscrito mediante ésta URL: /Inscripcion?idEvento=101, el valor "101" es puesto en el <input> escondido que es parte de los datos de la inscripción, y éso llena el campo "IdEvento" aquí.
        return Created ();
    }

    //Pendiente: Manejo de errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
