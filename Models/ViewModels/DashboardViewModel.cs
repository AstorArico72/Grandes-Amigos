// Archivo: Models/ViewModels/DashboardViewModel.cs

namespace Grandes_Amigos.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int TotalEventos { get; set; }
        public List<Evento> EventosRecientes { get; set; } = new List<Evento>();
    }
}
