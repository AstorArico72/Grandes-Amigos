using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

public class Inscrito {
    [Key]
    [Column("Num_Documento")]
    public int NumDocumento {get; set;} // ✔ Ahora EF puede mapearlo
    [Column("Tipo_Documento")]

    public string TipoDocumento {get; set;}
    [Column("Correo")]
    public string Correo {get; set;}
    [Column("Teléfono")]
    public string Teléfono {get; set;}
    [Column("Asociación")]
    public string Asociación { get; set; }
    [Column("Nombre")]
    public string Nombre {get; set;}

    // ✔ Constructor requerido por EF Core
    public Inscrito() { }

    // ✔ Constructor personalizado opcional
    public Inscrito(int documento, string nombre, string tipoDocumento, string correo, string telefono, string asociacion) {
        NumDocumento = documento;
        TipoDocumento = tipoDocumento;
        Nombre = nombre;
        Correo = correo;
        Teléfono = telefono;
        Asociación = asociacion;
    }
}
