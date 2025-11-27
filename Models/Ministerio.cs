using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Ministerios")]
public class Ministerio
{
    [Key]
    [Column("ID")]
    public int ID { get; set; }

    [Required(
        ErrorMessage = "Es necesario un nombre para el ministerio.",
        AllowEmptyStrings = false
    )]
    public string Nombre { get; set; }

    // Constructor vacío requerido por EF Core
    public Ministerio() { }

    // Constructor útil para lógica de negocio o tests
    public Ministerio(int id, string nombre)
    {
        ID = id;
        Nombre = nombre;
    }
}
