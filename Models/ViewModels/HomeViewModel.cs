namespace Grandes_Amigos.Models.ViewModels;

public class HomeViewModel
{
    public List<Noticia> Deporte { get; set; } = new();
    public List<Noticia> Cultura { get; set; } = new();
    public List<Noticia> Salud { get; set; } = new();

    public List<EventoConMinisterioViewModel> Eventos { get; set; } = new();
    public List<EventoConMinisterioViewModel> EventosDestacados { get; set; } = new();
}
