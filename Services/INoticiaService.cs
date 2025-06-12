using Grandes_Amigos.Models;

public interface INoticiaService
{
    Task<List<Noticia>> GetNoticiasDeporteAsync();
    Task<List<Noticia>> GetNoticiasCulturaAsync();
    Task<List<Noticia>> GetNoticiasSaludAsync();
}
