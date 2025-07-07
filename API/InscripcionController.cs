using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Inscripciones")]
public class InscripcionController : Controller
{
    // InscripcionController.cs
    //
    // CRUD de inscripciones
    // Patrón: [host]/Api/Inscripciones
    private readonly ILogger<InscripcionController> _logger;
    private ContextoDb Contexto;

    public InscripcionController(ILogger<InscripcionController> logger, ContextoDb contexto)
    {
        _logger = logger;
        Contexto = contexto;
    }

    [AllowAnonymous]
    [HttpGet("Inscribirse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult FormularioInscripcion([FromQuery] int IdEvento)
    {
        //Éste método carga el formulario para la inscripción.
        //Cada evento en /Eventos/{id} tendrá un botón que enlaza a ésta URL, con el ID del evento en el querystring.
        //En la vista, tiene que haber un <input> "escondido" (type="hidden") que tiene el nombre "IdEvento".
        //Ése campo es llenado por el valor del querystring.
        //El querystring es importante, ver explicación debajo.
        return View();
    }

    [AllowAnonymous]
    [HttpPost("Nueva")] //Pendiente: Decidir si será un formulario POST o un enlace GET con poca interacción del usuario.
    [SwaggerOperation(
        Summary = "Crea una nueva inscripción.",
        Description = "Crea una nueva entrada en la tabla `Inscripciones`, tomando como parámetros los ID del inscrito y del evento. El campo `IdEvento` es llenado por el querystring en `FormularioInscripcion`. Es decir, si uno se ha inscrito mediante ésta URL: `/Inscripcion?idEvento=101`, el valor `101` es puesto en el `<input>` escondido que es parte de los datos de la inscripción, y éso llena el campo `IdEvento` aquí."
    )]
    [SwaggerResponse(201, "Se cargó la nueva inscripción a la base de datos exitosamente.")]
    [SwaggerResponse(
        400,
        "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal."
    )]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public async Task<IActionResult> NuevaInscripcion([FromForm] int IdEvento)
    {
        //Validación para confirmar si llegaron valores correctos.
        //Usuario recibido de las Claims.
        Usuario? UsuarioEncontrado = await Contexto.Usuarios.FindAsync(
            User.Claims.Where(claim => claim.Type == "NumDocumento").First().Value
        );
        Evento? EventoEncontrado = await Contexto.Eventos.FindAsync(IdEvento);

        if (UsuarioEncontrado != null && EventoEncontrado != null)
        {
            Inscripción NuevaInscripcion = new Inscripción(
                EventoEncontrado.ID,
                UsuarioEncontrado.NumDocumento
            );
            try
            {
                Contexto.Inscripciones.Add(NuevaInscripcion);
                await Contexto.SaveChangesAsync();
                return Created();
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, ex);
            }
        }
        else
        {
            return BadRequest("Evento o inscrito inválidos.");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> NuevaInscripcionJson([FromBody] InscripcionDto dto)
    {
        if (dto == null || dto.id_Evento <= 0 || dto.id_Inscrito <= 0)
            return BadRequest(new { message = "Datos inválidos." });

        var evento = await Contexto.Eventos.FindAsync(dto.id_Evento);
        var usuario = await Contexto.Usuarios.FindAsync(dto.id_Inscrito); // dto.id_Inscrito == NumDocumento

        if (evento == null || usuario == null)
            return BadRequest(new { message = "Evento o usuario no encontrado." });

        // Verificar si ya existe inscripción
        bool yaInscripto = Contexto.Inscripciones.Any(i =>
            i.ID_Evento == dto.id_Evento && i.ID_Inscrito == dto.id_Inscrito
        );
        if (yaInscripto)
            return BadRequest(new { message = "Ya estás inscrito en este evento." });

        var insc = new Inscripción(dto.id_Evento, dto.id_Inscrito);
        try
        {
            Contexto.Inscripciones.Add(insc);
            await Contexto.SaveChangesAsync();
            return StatusCode(201, new { message = "Inscripción exitosa." });
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("PorEvento")]
    [SwaggerResponse(200, "Hay al menos una inscripción asociada al evento.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(204, "No hay inscripciones asociadas al evento, pero el evento existe.")]
    [SwaggerResponse(400, "El evento no existe.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    [SwaggerOperation(
        Summary = "Lista las inscripciones de un evento.",
        Description = "Tomando el ID del evento del querystring, ésta función lista todas las entradas en la tabla `Inscripciones` donde el campo `ID_Evento` coincida."
    )]
    public IActionResult InscripcionesPorEvento([FromQuery] int IdEvento)
    {
        try
        {
            Evento? EventoEncontrado = Contexto.Eventos.Find(IdEvento);
            List<Inscripción>? Inscripciones = Contexto
                .Inscripciones.Where(inscripcion => inscripcion.ID_Evento == IdEvento)
                .ToList();
            if (EventoEncontrado == null)
            {
                return BadRequest("El evento seleccionado no existe.");
            }
            else if (Inscripciones.Count() == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(Inscripciones);
            }
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("Existe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ExisteInscripcion([FromQuery] int eventoId, [FromQuery] int usuarioId)
    {
        if (eventoId <= 0 || usuarioId <= 0)
            return BadRequest(new { message = "Parámetros inválidos." });

        bool inscripto = Contexto.Inscripciones.Any(i =>
            i.ID_Evento == eventoId && i.ID_Inscrito == usuarioId
        );

        return Ok(new { inscripto });
    }
}
