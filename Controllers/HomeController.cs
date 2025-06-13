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
        var model = new HomeViewModel
        {
            Deporte = await _noticiaService.GetNoticiasDeporteAsync(),
            Cultura = await _noticiaService.GetNoticiasCulturaAsync(),
            Salud = await _noticiaService.GetNoticiasSaludAsync(),
            Eventos = await _context.Eventos.OrderBy(e => e.Fecha).ToListAsync(),
        };

        return View("~/Views/Public/Index.cshtml", model);
    }
}
