using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Noticias")]
public class Noticia
{
    [Key]
    public int ID { get; set; }

    [Required]
    public string? Título { get; set; }
    public string? Contenido { get; set; }

    [Required]
    public string? Autor { get; set; }

    [Required]
    public string? Categoria { get; set; } // Nueva propiedad para categorizar

    [Required]
    public string? Enlace { get; set; }

    public string? ImagenUrl { get; set; }

    [Required]
    [Column("Fecha_Publicación")]
    public DateTime FechaPublicación { get; set; }

    public Noticia() { }
}
