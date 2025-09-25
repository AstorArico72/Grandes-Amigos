using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Eventos")]
public class EventosController : Controller
{
    // EventosController.cs
    //
    // CRUD de eventos
    // Patrón: [host]/Api/Eventos
    private readonly ILogger<EventosController> _logger;
    private ContextoDb Contexto;

    public EventosController(ILogger<EventosController> logger, ContextoDb contexto)
    {
        _logger = logger;
        Contexto = contexto;
    }

    [AllowAnonymous]
    [HttpGet("Lista")]
    [SwaggerOperation(
        Summary = "Lista todos los eventos.",
        Description = "Lee todas las entradas de la tabla `Eventos`."
    )]
    [SwaggerResponse(200, "Hay al menos una entrada en la tabla 'Eventos'.")]
    [SwaggerResponse(204, "La tabla 'Eventos' está vacía.")]
    public IActionResult VerTodos()
    {
        List<Evento> Eventos = Contexto.Eventos.ToList();
        return Ok(Eventos);
    }

    [AllowAnonymous]
    [HttpGet("PorMinisterio/{Ministerio}")]
    [SwaggerOperation(
        Summary = "Lista los eventos de un ministerio.",
        Description = "Lee todas las entradas de la tabla `Eventos` que están asociadas al ministerio especificado en la ruta."
    )]
    [SwaggerResponse(200, "Hay al menos una entrada.")]
    [SwaggerResponse(404, "El ministerio no existe; o existe, pero no hay eventos asociados.")]
    public IActionResult PorMinisterio([FromRoute] string Ministerio) {
        Ministerio? min = Contexto.Ministerios.FirstOrDefault(m => m.Nombre.ToLower() == Ministerio.ToLower()); //Ésto permite entrarlo tanto con o sin mayúsculas.
        
        if (min == null)
        {
            return NotFound("El ministerio pedido no existe.");
        }

        List<Evento?> Eventos = Contexto.Eventos.Where(e => e.ID_Ministerio == min.ID).ToList<Evento?>();

        if (Eventos.IsNullOrEmpty())
        {
            return NotFound("El ministerio pedido no tiene eventos.");
        }
        else
        {
            return Ok(Eventos);
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna un evento.",
        Description = "Toma el ID del evento de la ruta; y si un evento con ése ID existe en la base de datos, lo lee."
    )]
    [SwaggerResponse(200, "El evento existe en la base de datos.")]
    [SwaggerResponse(404, "El evento no existe en la base de datos.")]
    public IActionResult VerEvento([FromRoute] int id)
    {
        var evento = Contexto.Eventos.Find(id);
        if (evento == null)
            return NotFound();

        // Buscar el ministerio relacionado
        var ministerio = Contexto.Ministerios.Find(evento.ID_Ministerio);

        // Devolver el evento + nombre del ministerio
        return Ok(
            new
            {
                id = evento.ID,
                título = evento.Título,
                descripcion = evento.Descripción,
                fecha = evento.Fecha,
                foto = evento.Foto,
                id_Ministerio = evento.ID_Ministerio,
                ministerioNombre = ministerio != null ? ministerio.Nombre : null,
            }
        );
    }

    [AllowAnonymous] //Autorización temporalmente quitada.
    [HttpPost("Nuevo")]
    [SwaggerOperation(
        Summary = "Crea un nuevo evento.",
        Description = "Crea una entrada en la tabla `Eventos`, tomando el cuerpo del pedido como parámetro. Ve a `/Models/Evento.cs` para saber qué entra aquí."
    )]
    [SwaggerResponse(201, "Se cargó el nuevo evento a la base de datos exitosamente.")]
    [SwaggerResponse(
        400,
        "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal."
    )]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult NuevoEvento([FromForm] Evento NuevoEvento)
    {
        //Ésto asume que los datos llegan de un formulario de tipo "x-www-form-urlencoded".

        Ministerio? ministerio = Contexto.Ministerios.Find(NuevoEvento.ID_Ministerio);

        //Ésto asegura que el campo "ministerio" no apunte a un ministerio que no existe.
        if (ministerio == null)
        {
            ModelState.AddModelError(
                nameof(NuevoEvento.ID_Ministerio),
                "El ministerio con ID #" + NuevoEvento.ID_Ministerio + " no existe."
            );
        }

        //Ésto asegura que los eventos sólo puedan crearse con fechas futuras.
        if (DateTime.Compare(NuevoEvento.Fecha, DateTime.Today.AddDays(1)) <= 0)
        {
            //Pendiente: Consultar si los eventos tienen fecha de inicio y fecha de fin.
            ModelState.AddModelError(
                nameof(NuevoEvento.Fecha),
                "Los eventos deben crearse con al menos un día de anticipación."
            );
        }

        try
        {
            if (ModelState.IsValid)
            {
                Contexto.Eventos.Add(NuevoEvento);
                Contexto.SaveChanges();
                return Created();
            }
            else
            {
                // Pendiente: Reemplazar ésto por una página de error comprensiva en la implementación para los clientes.
                List<string> ErroresModelo = new List<string>();
                var errores = ModelState.Values.SelectMany(value => value.Errors);
                foreach (var item in errores)
                {
                    ErroresModelo.Add(item.ErrorMessage);
                }
                return BadRequest(
                    "Estado de modelo inválido:\n" + string.Join("\n", ErroresModelo)
                );
            }
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Authorize(Policy = "Ministerio")]
    [HttpPut("Editar")]
    [SwaggerOperation(
        Summary = "Edita un evento.",
        Description = "Tomando el cuerpo del pedido, edita una entrada de la tabla 'Eventos', donde el ID del evento coincida con el ID especificado en el cuerpo."
    )]
    [SwaggerResponse(200, "El evento fue actualizado en la base de datos con éxito.")]
    [SwaggerResponse(
        400,
        "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal."
    )]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult EditarEvento([FromForm] Evento EventoEditado)
    {
        //Ésta función es para editar el evento como un todo.
        //Ésta función debería ser llamada desde un formulario parecido o idéntico al de crear eventos.
        //Pendiente: Tal vez implementar una versión más ligera de ésta función para cambiar un sólo campo (por ejemplo, la fecha) que use HTTP PATCH en lugar de PUT

        //Ésto sirve para revisar si llegó un ID erróneo. Tal vez sea necesario quitarlo.
        Evento? EventoSeleccionado = Contexto.Eventos.Find(EventoEditado.ID);
        if (EventoSeleccionado != null)
        {
            if (EventoEditado.Fecha <= DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(EventoEditado.Fecha),
                    "Los eventos deben crearse con al menos un día de anticipación."
                );
            }
            try
            {
                if (ModelState.IsValid)
                {
                    EventoSeleccionado.Título = EventoEditado.Título;
                    EventoSeleccionado.Fecha = EventoEditado.Fecha;
                    EventoSeleccionado.Descripción = EventoEditado.Descripción;
                    EventoSeleccionado.Foto = EventoEditado.Foto;
                    Contexto.SaveChanges();
                    Contexto.Entry(EventoSeleccionado).State = EntityState.Modified;
                    return Ok();
                }
                else
                {
                    List<string> ErroresModelo = new List<string>();
                    var errores = ModelState.Values.SelectMany(value => value.Errors);
                    foreach (var item in errores)
                    {
                        ErroresModelo.Add(item.ErrorMessage);
                    }
                    return BadRequest(
                        "Estado de modelo inválido:\n" + string.Join("\n", ErroresModelo)
                    );
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, ex);
            }
        }
        else
        {
            return BadRequest("El evento seleccionado no existe.");
        }
    }

    [Authorize(Policy = "Ministerio")]
    [HttpDelete("Borrar/{id}")]
    [SwaggerOperation(
        Summary = "Borra un evento.",
        Description = "Borra una entrada de la tabla 'Eventos' tomando el ID de la ruta como parámetro. Si el evento existe, es borrado."
    )]
    [SwaggerResponse(200, "El evento fué borrado con éxito.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(400, "El evento seleccionado no existe, es decir, el ID es inválido.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult BorrarEvento([FromRoute] int id)
    {
        //Es importante que haya una confirmación, como "¿Está seguro de borrar éste evento? Una vez hecho, no hay vuelta atrás." antes de acceder a éste endpoint.
        Evento? EventoSeleccionado = Contexto.Eventos.Find(id);
        if (EventoSeleccionado != null)
        {
            try
            {
                Contexto.Remove(EventoSeleccionado);
                Contexto.SaveChanges();
                return Ok();
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, ex);
            }
        }
        else
        {
            return BadRequest("El evento seleccionado no existe.");
        }
    }

    [Authorize(Policy = "Usuario")]
    [HttpGet("MisEventos")]
    [SwaggerOperation(
        Summary = "Lista los eventos asociados a los que se ha inscrito el usuario.",
        Description = "Tomando la tabla 'Inscripciones' y las credenciales de usuario como referencia, filtra las entradas en la tabla 'Eventos', primero revisando en la tabla 'Inscripciones' si hay entradas con el ID del usuario, y luego tomando el ID del evento de la misma entrada."
    )]
    [SwaggerResponse(200, "El usuario está inscrito a al menos un evento.")]
    [SwaggerResponse(400, "El usuario no está inscrito a ningún evento.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult MisEventos()
    {
        string IdUsuario = User.Claims.First(claim => claim.Type == "NumDocumento").Value;
        Usuario? UsuarioEncontrado = Contexto.Usuarios.Find(Int32.Parse(IdUsuario));
        
        if (UsuarioEncontrado == null) //Validación para casos borde.
        {
            return Unauthorized("Usuario inválido.");
        }
        else
        {
            List<Inscripción> inscripciones = Contexto.Inscripciones.Where(item => item.ID_Inscrito == UsuarioEncontrado.NumDocumento).ToList();
            List<Evento> eventos = new List<Evento>();
            inscripciones.ForEach(item =>
            {
                Evento? encontrado = Contexto.Eventos.Find(item.ID_Evento);
                if (encontrado != null)
                {
                    eventos.Add(encontrado);
                }
            });
            if (eventos.Count == 0)
            {
                return BadRequest("No hay inscripciones.");
            }
            else
            {
                return Ok(eventos);
            }
        }
    }

    //Pendiente: Manejo de errores
}
