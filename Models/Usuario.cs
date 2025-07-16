using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

//Pendiente: Cambiar el nombre de ésta tabla. Creo que el nombre "Inscritos" no refleja la función.

public class Usuario
{
    [Key]
    [Required(ErrorMessage = "Es necesario el número de documento.", AllowEmptyStrings = false)]
    [Column("Num_Documento")]
    public int NumDocumento { get; set; }

    [Required(
        ErrorMessage = "Es necesario indicar el tipo de documento.",
        AllowEmptyStrings = false
    )]
    [StringLength(maximumLength: 3, MinimumLength = 2)]
    [Column("Tipo_Documento")]
    public string TipoDocumento { get; set; }

    //Pendiente: Consultar cuáles campos son obligatorios, si el correo o el teléfono.
    [Required(
        ErrorMessage = "Es necesaria una dirección de correo electrónico.",
        AllowEmptyStrings = false
    )]
    public string Correo { get; set; }

    [Required(ErrorMessage = "Es necesario un número de teléfono.")]
    /* Explicación de los límites en los números:
    *
    * "11 0123 4567" ó "351 012 3456" (sin los espacios), son 10 caracteres.
    */
    [StringLength(maximumLength: 10, MinimumLength = 10)]
    public string Teléfono { get; set; }

    [Required(
        ErrorMessage = "Es necesario estar registrado en una asociación.",
        AllowEmptyStrings = false
    )]
    public string Asociación { get; set; }

    [Required(
        ErrorMessage = "Es necesario registrarse con nombre y apellido.",
        AllowEmptyStrings = false
    )]
    //Pendiente: Consultar si es necesario separar nombre de apellido.
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Es necesario usar una clave.", AllowEmptyStrings = false)]
    //Pendiente: Consultar si es necesario separar nombre de apellido.
    public string Clave { get; set; }

    // ✔ Constructor requerido por EF Core
    public Usuario() { }

    // ✔ Constructor personalizado opcional
    public Usuario(
        int documento,
        string nombre,
        string tipoDocumento,
        string correo,
        string telefono,
        string asociacion
    )
    {
        NumDocumento = documento;
        TipoDocumento = tipoDocumento;
        Nombre = nombre;
        Correo = correo;
        Teléfono = telefono;
        Asociación = asociacion;
    }
}
