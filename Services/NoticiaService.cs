using Grandes_Amigos.Models;

public class NoticiaService : INoticiaService
{
    public Task<List<Noticia>> GetNoticiasDeporteAsync()
    {
        return Task.FromResult(
            new List<Noticia>
            {
                new Noticia
                {
                    Título = "Clases abiertas en el Polideportivo",
                    Contenido =
                        "La Secretaría de Deporte organiza clases gratuitas para adultos mayores.",
                    Autor = "Ministerio de Deporte",
                    Enlace = "#",
                    FechaPublicación = DateTime.Now.AddDays(-1),
                },
                new Noticia
                {
                    Título = "Caminatas saludables en el Parque",
                    Contenido = "Se realizan caminatas semanales con acompañamiento profesional.",
                    Autor = "Bienestar Activo",
                    Enlace = "#",
                    FechaPublicación = DateTime.Now.AddDays(-2),
                },
            }
        );
    }

    public Task<List<Noticia>> GetNoticiasCulturaAsync()
    {
        return Task.FromResult(
            new List<Noticia>
            {
                new Noticia
                {
                    Título = "Muestra de Arte Intergeneracional",
                    Contenido = "Adultos mayores y jóvenes comparten espacio creativo.",
                    Autor = "Ministerio de Cultura",
                    Enlace = "#",
                    FechaPublicación = DateTime.Now.AddDays(-3),
                },
            }
        );
    }

    public Task<List<Noticia>> GetNoticiasSaludAsync()
    {
        return Task.FromResult(
            new List<Noticia>
            {
                new Noticia
                {
                    Título = "Jornada gratuita de control de presión arterial",
                    Contenido = "Este viernes en todos los centros de salud. ¡Participá!",
                    Autor = "Ministerio de Salud",
                    Enlace = "#",
                    FechaPublicación = DateTime.Now.AddDays(-2),
                },
            }
        );
    }
}
