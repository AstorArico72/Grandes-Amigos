using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Eventos")]
public class Evento
{
    [Key]
    public int ID { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Falta un título para el evento.", AllowEmptyStrings = false)]
    public string? Título { get; set; }

    [Required(ErrorMessage = "Falta una descripción para el evento.", AllowEmptyStrings = false)]
    public string? Descripción { get; set; }

    [Required]
    [ForeignKey("Ministerio")]
    public int ID_Ministerio { get; set; }

    // navegación opcional para evitar validación automática
    public Ministerio? Ministerio { get; set; }

    // La foto la guardamos desde el controller al procesar IFormFile.
    // Si la quieres obligatoria, valida después de guardar el archivo.
    public string? Foto { get; set; }

    public Evento() { }

    public Evento(string titulo, string descripcion, DateTime fecha, int ministerio)
    {
        Título = titulo;
        Descripción = descripcion;
        Fecha = fecha;
        ID_Ministerio = ministerio;
    }
}
