using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

public class Usuario
{
    [Key]
    public int ID { get; set; } // ✔ EF puede leer/escribir

    // 🚫 El atributo [Index(IsUnique = true)] solo se usa desde EF Fluent API. Si querés que sea único, lo hacemos en el DbContext.
    public string NombreUsuario { get; set; }
    public string Clave { get; set; }

    [ForeignKey("Ministerio")]
    public int IdMinisterio { get; set; }

    // ✔ Constructor requerido por EF Core
    public Usuario() { }

    // ✔ Constructor personalizado opcional
    public Usuario(int id, string nombre, string clave, int ministerio)
    {
        ID = id;
        NombreUsuario = nombre;
        Clave = clave;
        IdMinisterio = ministerio;
    }
}
