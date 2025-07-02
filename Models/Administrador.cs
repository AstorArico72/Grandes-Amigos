using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Administradores")]
public class Administrador {
    [Key]
    public int ID { get; set; } // ✔ EF puede leer/escribir

    // 🚫 El atributo [Index(IsUnique = true)] solo se usa desde EF Fluent API. Si querés que sea único, lo hacemos en el DbContext.
    [Required]
    [Column("Nombre_Usuario")]
    public string NombreUsuario { get; set; }

    [Required]
    public string Clave { get; set; }

    [ForeignKey("Ministerio")]
    [Column("ID_Ministerio")]
    public int IdMinisterio { get; set; }

    // ✔ Constructor requerido por EF Core
    public Administrador() { }

    // ✔ Constructor personalizado opcional
    public Administrador(int id, string nombre, string clave, int ministerio)
    {
        ID = id;
        NombreUsuario = nombre;
        Clave = clave;
        IdMinisterio = ministerio;
    }
}
