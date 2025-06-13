using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Table("Eventos")]
public class Evento
{
    [Key]
    public int ID { get; set; } //Ahora EF puede asignar el valor de la DB

    [Required]
    //Pendiente: Validar que la fecha sea al menos un día después del día corriente mediante anotaciones.
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Falta un título para el evento.", AllowEmptyStrings = false)]
    public string? Título { get; set; }

    [Required(ErrorMessage = "Falta una descripción para el evento.", AllowEmptyStrings = false)]
    public string? Descripción { get; set; }

    [Required]
    [ForeignKey("Ministerio-Evento")]
    public int ID_Ministerio { get; set; }

    [Required(
        ErrorMessage = "Es necesario incluir una foto." /*¿O no? Hay que consultar éso.*/
        ,
        AllowEmptyStrings = false
    )]
    public string? Foto { get; set; }

    //Constructor requerido por EF Core
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
