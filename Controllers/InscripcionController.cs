using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Controllers;

[ApiController]
[AllowAnonymous] //El uso de [AllowAnonymous] es porque éste endpoint es usado por los visitantes del sitio, quienes, como se ha discutido antes, no necesitan registrarse.
[Route("/Api/Inscripciones")]
public class InscripcionController : Controller {
    // InscripcionController.cs
    // 
    // CRUD de inscripciones
    // Patrón: [host]/Api/Inscripciones
    private readonly ILogger<InscripcionController> _logger;
    private ContextoDb Contexto;

    public InscripcionController(ILogger<InscripcionController> logger, ContextoDb contexto) {
        _logger = logger;
        Contexto = contexto;
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

    [HttpPost("Nueva")]
    public async Task<IActionResult> NuevaInscripcion ([FromForm]int IdInscrito, [FromForm]int IdEvento) {
        //Validación para confirmar si llegaron valores correctos.
        Inscrito? InscritoEncontrado = await Contexto.Inscritos.FindAsync (IdInscrito);
        Evento? EventoEncontrado = await Contexto.Eventos.FindAsync (IdEvento); //Éste campo es llenado por el querystring en "FormularioInscripcion".
        //Es decir, si uno se ha inscrito mediante ésta URL: /Inscripcion?idEvento=101, el valor "101" es puesto en el <input> escondido que es parte de los datos de la inscripción, y éso llena el campo "IdEvento" aquí.

        if (InscritoEncontrado != null && EventoEncontrado != null) {
            Inscripción NuevaInscripción = new Inscripción (EventoEncontrado.ID, InscritoEncontrado.NumDocumento);
            try {
                Contexto.Inscripciones.Add (NuevaInscripción);
                Contexto.Entry (NuevaInscripción).State = EntityState.Detached;
                return Created ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("Evento o inscrito inválidos.");
        }
    }

    //Pendiente: Manejo de errores
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
