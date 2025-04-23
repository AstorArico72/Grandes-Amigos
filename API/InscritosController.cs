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
        if (ModelState.IsValid) { //Pendiente: Usar anotaciones para determinar la propiedad ModelState.IsValid
            try {
                Contexto.Inscritos.Add (NuevoInscrito);
                await Contexto.SaveChangesAsync ();
                return Created ();
            } catch (MySqlException ex) {
                return StatusCode (500, ex);
            }
        } else {
            return BadRequest ("Estado de modelo inválido.");
        }
    }
}