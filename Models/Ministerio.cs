using System.ComponentModel.DataAnnotations;

namespace Grandes_Amigos.Models;

public class Ministerio
{
    [Key]
    public int ID { get; set; } // ✅ Ahora EF puede mapearlo correctamente

    public string Nombre { get; set; }

    // ✅ Constructor vacío requerido por EF Core
    public Ministerio() { }

    // ✅ Constructor útil para lógica de negocio o tests
    public Ministerio(int id, string nombre)
    {
        ID = id;
        Nombre = nombre;
    }
}
