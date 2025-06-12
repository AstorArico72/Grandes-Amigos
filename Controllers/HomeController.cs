using Grandes_Amigos.Models;
using Grandes_Amigos.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly INoticiaService _noticiaService;

    public HomeController(INoticiaService noticiaService)
    {
        _noticiaService = noticiaService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new NoticiasViewModel
        {
            Deporte = await _noticiaService.GetNoticiasDeporteAsync(),
            Cultura = await _noticiaService.GetNoticiasCulturaAsync(),
            Salud = await _noticiaService.GetNoticiasSaludAsync(),
        };

        return View("~/Views/Public/Index.cshtml", model);
    }
}
