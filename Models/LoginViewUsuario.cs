using System.ComponentModel.DataAnnotations;

namespace Grandes_Amigos.Models;

public class LoginViewUsuario {
    [Required(ErrorMessage = "Debe ingresar su número de documento.")]
    public string? Identificador { get; set; }
    /**
    * El "identificador" cumple la función de Usuario.NumDocumento y Admin.NombreUsuario.
    */

    [Required(ErrorMessage = "Debe ingresar su clave.")]
    public string? Clave { get; set; }
}
