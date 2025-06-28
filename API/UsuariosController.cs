using System.Security.Cryptography;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Usuario")]
public class InscritosController : Controller
{
    private readonly ContextoDb Contexto;
    private readonly IConfiguration Config;

    public InscritosController(ContextoDb contexto, IConfiguration config)
    {
        Contexto = contexto;
        Config = config;
    }

    [AllowAnonymous]
    [HttpPost("Nuevo")]
    public async Task<IActionResult> RegistrarInscrito([FromForm] Inscrito nuevo)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos requeridos.");

        try
        {
            // Hashear clave
            nuevo.Clave = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: nuevo.Clave,
                    salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 4096,
                    numBytesRequested: 256 / 8
                )
            );

            await Contexto.Inscritos.AddAsync(nuevo);
            await Contexto.SaveChangesAsync();

            return Created();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Error al guardar el inscrito: {ex.Message}");
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginViewInscrito login)
    {
        if (!ModelState.IsValid)
            return BadRequest("Faltan datos.");

        var claveHasheada = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: login.Clave,
                salt: System.Text.Encoding.UTF8.GetBytes(Config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 4096,
                numBytesRequested: 256 / 8
            )
        );

        var inscrito = await Contexto.Inscritos.FirstOrDefaultAsync(i =>
            i.NumDocumento == login.NumDocumento
        );

        if (inscrito == null || inscrito.Clave != claveHasheada)
        {
            return BadRequest("DNI o clave incorrectos.");
        }

        // Guardamos nombre y DNI en la sesión (si está habilitada)
        HttpContext.Session.SetString("Nombre", inscrito.Nombre);
        HttpContext.Session.SetInt32("DNI", inscrito.NumDocumento);

        return Ok("Inicio de sesión exitoso.");
    }
}
