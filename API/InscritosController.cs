using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;

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
}