using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly INoticiaService _noticiaService;
    private readonly ContextoDb _context;

    // Constructor que inyecta tanto el servicio de noticias como la base de datos
    public HomeController(INoticiaService noticiaService, ContextoDb context)
    {
        _noticiaService = noticiaService;
        _context = context;
    }

    // Acción principal para la vista Index
    public async Task<IActionResult> Index()
    {
        var eventosConMinisterios = await _context
            .Eventos.Include(e => e.Ministerio)
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoConMinisterioViewModel
            {
                Evento = e,
                NombreMinisterio = e.Ministerio.Nombre,
            })
            .ToListAsync();

        var eventosDestacados = await _context
            .Eventos.Include(e => e.Ministerio)
            .OrderBy(e => e.Fecha)
            .Take(3)
            .Select(e => new EventoConMinisterioViewModel
            {
                Evento = e,
                NombreMinisterio = e.Ministerio.Nombre,
            })
            .ToListAsync();

        var model = new HomeViewModel
        {
            Deporte = await _noticiaService.GetNoticiasDeporteAsync(),
            Cultura = await _noticiaService.GetNoticiasCulturaAsync(),
            Salud = await _noticiaService.GetNoticiasSaludAsync(),
            Eventos = eventosConMinisterios,
            EventosDestacados = eventosDestacados,
        };

        return View("~/Views/Public/Index.cshtml", model);
    }
}
