using System.Security.Cryptography;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Admin")]
public class InscritosController : Controller {
    private readonly ContextoDb Contexto;
    private readonly IConfiguration Config;

    public InscritosController(ContextoDb contexto, IConfiguration config)
    {
        Contexto = contexto;
        Config = config;
    }

    [AllowAnonymous]
    [HttpPost("Nuevo")]
    public async Task<IActionResult> RegistrarAdmin([FromForm] Administrador nuevo)
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

            await Contexto.Admins.AddAsync(nuevo);
            await Contexto.SaveChangesAsync();

            return Created();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Error al guardar el administrador: {ex.Message}");
        }
    }

    
}
