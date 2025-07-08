namespace Grandes_Amigos.Models.ViewModels;

public class HomeViewModel
{
    public List<Noticia> Deporte { get; set; } = new List<Noticia>();
    public List<Noticia> Cultura { get; set; } = new List<Noticia>();
    public List<Noticia> Salud { get; set; } = new List<Noticia>();
    public List<EventoConMinisterioViewModel> Eventos { get; set; } =
        new List<EventoConMinisterioViewModel>();
}
