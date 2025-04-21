using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

public class Inscrito
{
    [Key]
    public int NumDocumento { get; set; } // ✔ Ahora EF puede mapearlo

    public string TipoDocumento { get; set; }
    public string Correo { get; set; }
    public string Teléfono { get; set; }
    public string Asociación { get; set; }

    // ✔ Constructor requerido por EF Core
    public Inscrito() { }

    // ✔ Constructor personalizado opcional
    public Inscrito(
        int documento,
        string tipoDocumento,
        string correo,
        string telefono,
        string asociacion
    )
    {
        NumDocumento = documento;
        TipoDocumento = tipoDocumento;
        Correo = correo;
        Teléfono = telefono;
        Asociación = asociacion;
    }
}
