using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Grandes_Amigos.Api;

[ApiController]
[AllowAnonymous]
[Route("/Api/Eventos")]
public class EventosController : Controller {
    // EventosController.cs
    // 
    // CRUD de eventos
    // Patrón: [host]/Api/Eventos
    private readonly ILogger<EventosController> _logger;
    private ContextoDb Contexto;

    public EventosController(ILogger<EventosController> logger, ContextoDb contexto) {
        _logger = logger;
        Contexto = contexto;
    }

    [HttpGet("Lista")]
    public IActionResult VerTodos() {
        List<Evento> Eventos = Contexto.Eventos.ToList ();
        return Ok (Eventos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> VerEvento([FromRoute] int id) {
        //Éste método carga la vista de detalles.
        //Es importante que las vistas de éste endpoint tengan un botón que enlace a /Inscripcion, con el ID del evento cargado en el querystring.
        //Es decir, algo como /Inscripcion?idEvento={id}
        //Ve a InscripcionController.cs para saber por qué.
        Evento? EventoEncontrado = await Contexto.Eventos.FindAsync (id);
        if (EventoEncontrado == null) {
            return NotFound ();
        } else {
            return Ok (EventoEncontrado);
        }
    }

    [AllowAnonymous]
    [HttpPost("Nuevo")]
    public async Task<IActionResult> NuevoEvento ([FromBody]Evento NuevoEvento) {
        //Ésto asume que los datos llegan de un formulario de tipo "x-www-form-urlencoded".

        Ministerio? ministerio = await Contexto.Ministerios.FindAsync (NuevoEvento.ID_Ministerio);

        //Ésto asegura que el campo "ministerio" no apunte a un ministerio que no existe.
        if (ministerio == null) {
            return BadRequest ("El ministerio con ID #" + NuevoEvento.ID_Ministerio + " no existe.");
        }

        //Ésto asegura que los eventos sólo puedan crearse con fechas futuras.
        if (NuevoEvento.Fecha <= DateTime.Today) {
            //Pendiente: Consultar si los eventos tienen fecha de inicio y fecha de fin.
            return BadRequest ("Los eventos deben crearse con al menos un día de anticipación.");
        }

        //Las dos validaciones debajo se explican solas.
        if (string.IsNullOrEmpty (NuevoEvento.Título)) {
            return BadRequest ("Falta un título para el evento.");
        }
        if (string.IsNullOrEmpty (NuevoEvento.Descripción)) {
            return BadRequest ("Falta una descripción para el evento.");
        }

        if (ModelState.IsValid) { //Pendiente: Usar anotaciones para determinar la propiedad ModelState.IsValid
            try {
                Contexto.Eventos.Add (NuevoEvento);
                await Contexto.SaveChangesAsync ();
                return Created ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("Estado de modelo inválido.");
        }
    }

    [Authorize]
    [HttpPut("Editar")]
    public async Task<IActionResult> EditarEvento ([FromForm]Evento EventoEditado) {
        //Ésta función es para editar el evento como un todo.
        //Ésta función debería ser llamada desde un formulario parecido o idéntico al de crear eventos.
        //Pendiente: Tal vez implementar una versión más ligera de ésta función para cambiar un sólo campo (por ejemplo, la fecha) que use HTTP PATCH en lugar de PUT

        //Ésto sirve para revisar si llegó un ID erróneo. Tal vez sea necesario quitarlo.
        Evento? EventoSeleccionado = Contexto.Eventos.Find (EventoEditado.ID);
        if (EventoSeleccionado != null) {
            //Debajo se hacen las mismas validaciones que en POST /Eventos/Nuevo
            if (!string.IsNullOrEmpty(EventoEditado.Título)) {
                EventoSeleccionado.Título = EventoEditado.Título;
            } else {
                return BadRequest ("Falta un título para el evento.");
            }

            if (!string.IsNullOrEmpty(EventoEditado.Descripción)) {
                EventoSeleccionado.Descripción = EventoEditado.Descripción;
            } else {
                return BadRequest ("Falta una descripción para el evento.");
            }

            if (EventoEditado.Fecha > DateTime.Today) {await Contexto.SaveChangesAsync ();
                EventoSeleccionado.Fecha = EventoEditado.Fecha;
            } else {
                return BadRequest ("Los eventos deben crearse con al menos un día de anticipación.");
            }
            
            try {
                await Contexto.SaveChangesAsync ();
                Contexto.Entry (EventoSeleccionado).State = EntityState.Modified;
                return Ok ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("El evento seleccionado no existe.");
        }
    }

    [Authorize]
    [HttpDelete("Borrar/{id}")]
    public IActionResult BorrarEvento ([FromRoute]int id) {
        //Es importante que haya una confirmación, como "¿Está seguro de borrar éste evento? Una vez hecho, no hay vuelta atrás." antes de acceder a éste endpoint.
        Evento? EventoSeleccionado = Contexto.Eventos.Find (id);
        if (EventoSeleccionado != null) {
            try {
                Contexto.Remove (EventoSeleccionado);
                Contexto.SaveChangesAsync ();
                return Ok ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("El evento seleccionado no existe.");
        }
    }

    //Pendiente: Manejo de errores
}
