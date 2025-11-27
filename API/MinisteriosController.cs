using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Api
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Ministerios")]
    [Produces("application/json")]
    [Authorize(Policy = "Ministerio")]
    public class MinisteriosController : ControllerBase
    {
        private readonly ContextoDb _ctx;
        private readonly ILogger<MinisteriosController> _logger;

        public MinisteriosController(ContextoDb ctx, ILogger<MinisteriosController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet("Lista")]
        public async Task<IActionResult> Lista()
        {
            var ministerios = await _ctx
                .Ministerios.OrderBy(m => m.Nombre)
                .Select(m => new { id = m.ID, m.Nombre })
                .ToListAsync();

            return Ok(ministerios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var ministerio = await _ctx.Ministerios.FindAsync(id);
            if (ministerio == null)
                return NotFound();

            return Ok(
                new
                {
                    id = ministerio.ID,
                    ministerio.Nombre,
                }
            );
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Ministerio nuevo)
        {
            if (nuevo == null || string.IsNullOrWhiteSpace(nuevo.Nombre))
                return BadRequest("El nombre es obligatorio.");

            var entidad = new Ministerio
            {
                Nombre = nuevo.Nombre.Trim(),
            };

            await _ctx.Ministerios.AddAsync(entidad);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(Obtener), new { id = entidad.ID }, new
            {
                id = entidad.ID,
                entidad.Nombre,
            });
        }

        [HttpPut("Editar/{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] Ministerio dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            var ministerio = await _ctx.Ministerios.FindAsync(id);
            if (ministerio == null)
                return NotFound();

            ministerio.Nombre = dto.Nombre.Trim();

            await _ctx.SaveChangesAsync();

            return Ok(
                new
                {
                    id = ministerio.ID,
                    ministerio.Nombre,
                }
            );
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ministerio = await _ctx.Ministerios.FindAsync(id);
            if (ministerio == null)
                return NotFound();

            _ctx.Ministerios.Remove(ministerio);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
