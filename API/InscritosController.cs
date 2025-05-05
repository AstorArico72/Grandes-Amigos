using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;

namespace Grandes_Amigos.Api;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Inscritos")]
public class InscritosController : Controller {
    // InscritosController.cs
    // ```
    // CRUD de Inscritos
    // Patrón: [host]/Api/Inscritos
    private readonly ILogger<InscritosController> _logger;
    private ContextoDb Contexto;

    public InscritosController(ILogger<InscritosController> logger, ContextoDb contexto) {
        _logger = logger;
        Contexto = contexto;
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("Todos")]
    [SwaggerOperation(
        Summary = "Lista todos los inscritos.",
        Description = "Lee todas las entradas de la tabla `Inscritos`."
        )]
    [SwaggerResponse (200, "Hay al menos una entrada en la tabla `Inscritos`.")]
    [SwaggerResponse (204, "La tabla `Inscritos` está vacía.")]
    [SwaggerResponse (401, "Se accedió sin autorización.")]
    public IActionResult VerTodos () {
        List <Inscrito> inscritos = Contexto.Inscritos.ToList ();
        return Ok (inscritos);
    }

    [Authorize(Policy = "Ministerio")]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna un inscrito.",
        Description = "Toma el ID del inscrito de la ruta; y si un inscrito con ése ID existe en la base de datos, lo lee."
        )]
    [SwaggerResponse(200, "El inscrito existe en la base de datos.")]
    [SwaggerResponse(404, "El inscrito no existe en la base de datos.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    public IActionResult DetallesInscrito ([FromRoute] int id) {
        Inscrito? InscritoEncontrado = Contexto.Inscritos.Find (id);
        if (InscritoEncontrado == null) {
            return NotFound ();
        } else {
            return Ok (InscritoEncontrado);
        }
    }

    [Authorize (Policy = "Ministerio")]
    [HttpPost("Nuevo")]
    [SwaggerOperation (
        Summary = "Crea un nuevo inscrito.",
        Description = "Crea una entrada en la tabla `Inscritos`, tomando el cuerpo del pedido como parámetro. Ve a `/Models/Inscrito.cs` para saber qué entra aquí."
    )]
    [SwaggerResponse(201, "Se cargó el nuevo inscrito a la base de datos exitosamente.")]
    [SwaggerResponse(400, "Algún campo tiene un valor inválido. Lee la respuesta para saber qué falta o está mal.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public async Task<IActionResult> NuevoInscrito ([FromBody]Inscrito NuevoInscrito) {
        try {
            if (ModelState.IsValid) {
                Contexto.Inscritos.Add (NuevoInscrito);
                await Contexto.SaveChangesAsync ();
                return Created ();
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
    }

    [Authorize (Policy = "Ministerio")] //Pendiente: Implementar política separada para los usuarios.
    [HttpDelete("Borrar/{id}")]
    [SwaggerOperation(
        Summary = "Borra un inscrito.",
        Description = "Borra una entrada de la tabla 'Inscritos' tomando el ID de la ruta como parámetro. Si el inscrito existe, es borrado."
        )]
    [SwaggerResponse(200, "El inscrito fué borrado con éxito.")]
    [SwaggerResponse(401, "Se accedió sin autorización.")]
    [SwaggerResponse(400, "El inscrito seleccionado no existe, es decir, el ID es inválido.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    public IActionResult BorrarInscrito ([FromRoute]int id) {
        Inscrito? InscritoSeleccionado = Contexto.Inscritos.Find (id);
        if (InscritoSeleccionado != null) {
            try {
                Contexto.Remove (InscritoSeleccionado);
                Contexto.SaveChanges ();
                return Ok ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("El inscrito seleccionado no existe.");
        }
    }
}