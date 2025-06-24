using Grandes_Amigos.Models;
using Microsoft.EntityFrameworkCore;

public class NoticiaService : INoticiaService
{
    private readonly ContextoDb _contexto;

    public NoticiaService(ContextoDb contexto)
    {
        _contexto = contexto;
    }

    public async Task<List<Noticia>> GetNoticiasDeporteAsync()
    {
        return await _contexto
            .Noticias.Where(n => n.Categoria == "Fútbol")
            .OrderByDescending(n => n.FechaPublicación)
            .ToListAsync();
    }

    public async Task<List<Noticia>> GetNoticiasCulturaAsync()
    {
        return await _contexto
            .Noticias.Where(n => n.Categoria == "Sociedad")
            .OrderByDescending(n => n.FechaPublicación)
            .ToListAsync();
    }

    public async Task<List<Noticia>> GetNoticiasSaludAsync()
    {
        return await _contexto
            .Noticias.Where(n => n.Categoria == "Espectáculos")
            .OrderByDescending(n => n.FechaPublicación)
            .ToListAsync();
    }
}
