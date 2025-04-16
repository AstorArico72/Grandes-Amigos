using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

public class Inscrito {
    [Key]
    public int NumDocumento {get;}
    public string TipoDocumento {get; set;}
    public string Correo {get; set;}
    public string Teléfono {get; set;}
    // Es necesario averiguar si hay que hacer una tabla "asociación" y convertir éste campo en una clave foránea.
    public string Asociación {get; set;}

    public Inscrito (int documento, string tipoDocumento, string correo, string telefono, string asociacion) {
        NumDocumento = documento;
        TipoDocumento = tipoDocumento;
        Correo = correo;
        Teléfono = telefono;
        Asociación = asociacion;
    }
}