using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Table("Eventos")]
public class Evento
{
    [Key]
    public int ID { get; set; } // ✅ Ahora EF puede asignar el valor de la DB

    public DateTime Fecha { get; set; }
    public string Título { get; set; }
    public string Descripción { get; set; }

    [ForeignKey("Ministerio-Evento")]
    public int ID_Ministerio { get; set; }

    public string Foto { get; set; }

    // ✅ Constructor requerido por EF Core
    public Evento() { }

    // Constructor personalizado para crear objetos fácilmente
    public Evento(string titulo, string descripcion, DateTime fecha, int ministerio)
    {
        Título = titulo;
        Descripción = descripcion;
        Fecha = fecha;
        ID_Ministerio = ministerio;
    }
}
