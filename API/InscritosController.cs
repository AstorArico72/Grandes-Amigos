using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;

namespace Grandes_Amigos.Api;

[ApiController]
[Route("/Api/Inscritos")]
public class InscritosController : Controller {
    // InscritosController.cs
    // 
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
    public IActionResult VerTodos () {
        List <Inscrito> inscritos = Contexto.Inscritos.ToList ();
        return Ok (inscritos);
    }

    [HttpGet("{id}")]
    public IActionResult DetallesInscrito ([FromRoute] int id) {
        Inscrito? InscritoEncontrado = Contexto.Inscritos.Find (id);
        if (InscritoEncontrado == null) {
            return NotFound ();
        } else {
            return Ok (InscritoEncontrado);
        }
    }

    [AllowAnonymous] //Ésto es temporal, hasta que se implementen las políticas de acceso.
    [HttpPost("Nuevo")]
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

    [HttpDelete("Borrar/{id}")]
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