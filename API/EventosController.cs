using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Grandes_Amigos.Api;

[ApiController]
[AllowAnonymous] //Ésta anotación está aquí porque la autorización no está implementada.
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

    [HttpPost("Nuevo")]
    public async Task<IActionResult> NuevoEvento ([FromBody]Evento NuevoEvento) {
        //Ésto asume que los datos llegan de un formulario de tipo "x-www-form-urlencoded".

        Ministerio? ministerio = await Contexto.Ministerios.FindAsync (NuevoEvento.ID_Ministerio);

        //Ésto asegura que el campo "ministerio" no apunte a un ministerio que no existe.
        if (ministerio == null) {
            ModelState.AddModelError (nameof (NuevoEvento.ID_Ministerio), "El ministerio con ID #" + NuevoEvento.ID_Ministerio + " no existe.");
        }

        //Ésto asegura que los eventos sólo puedan crearse con fechas futuras.
        if (DateTime.Compare (NuevoEvento.Fecha, DateTime.Today.AddDays (1)) <= 0) {
            //Pendiente: Consultar si los eventos tienen fecha de inicio y fecha de fin.
            ModelState.AddModelError (nameof (NuevoEvento.Fecha), "Los eventos deben crearse con al menos un día de anticipación.");
        }

        try {
            if (ModelState.IsValid) {
                Contexto.Eventos.Add (NuevoEvento);
                await Contexto.SaveChangesAsync ();
                return Created ();
            } else {
                // Pendiente: Reemplazar ésto por una página de error comprensiva en la implementación para los clientes.
                List <string> ErroresModelo = new List<string> ();
                var errores = ModelState.Values.SelectMany (value => value.Errors);
                foreach (var item in errores) {
                    ErroresModelo.Add (item.ErrorMessage);
                }
                return BadRequest ("Estado de modelo inválido:\n" + string.Join ("\n", ErroresModelo));
            }
        } catch (MySqlException ex) {
            return StatusCode (500, ex);
        }
    }

    [HttpPut("Editar")]
    public async Task<IActionResult> EditarEvento ([FromForm]Evento EventoEditado) {
        //Ésta función es para editar el evento como un todo.
        //Ésta función debería ser llamada desde un formulario parecido o idéntico al de crear eventos.
        //Pendiente: Tal vez implementar una versión más ligera de ésta función para cambiar un sólo campo (por ejemplo, la fecha) que use HTTP PATCH en lugar de PUT

        //Ésto sirve para revisar si llegó un ID erróneo. Tal vez sea necesario quitarlo.
        Evento? EventoSeleccionado = Contexto.Eventos.Find (EventoEditado.ID);
        if (EventoSeleccionado != null) {
            if (EventoEditado.Fecha <= DateTime.Today) {
                ModelState.AddModelError (nameof (EventoEditado.Fecha), "Los eventos deben crearse con al menos un día de anticipación.");
            }
            try {
                if (ModelState.IsValid) {
                    EventoSeleccionado.Título = EventoEditado.Título;
                    EventoSeleccionado.Fecha = EventoEditado.Fecha;
                    EventoSeleccionado.Descripción = EventoEditado.Descripción;
                    EventoSeleccionado.Foto = EventoEditado.Foto;
                    await Contexto.SaveChangesAsync ();
                    Contexto.Entry (EventoSeleccionado).State = EntityState.Modified;
                    return Ok ();
                } else {
                    List <string> ErroresModelo = new List<string> ();
                    var errores = ModelState.Values.SelectMany (value => value.Errors);
                    foreach (var item in errores) {
                        ErroresModelo.Add (item.ErrorMessage);
                    }
                    return BadRequest ("Estado de modelo inválido:\n" + string.Join ("\n", ErroresModelo));
                }
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
                Contexto.SaveChanges ();
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
