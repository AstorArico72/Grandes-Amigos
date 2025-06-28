using System.ComponentModel.DataAnnotations;

namespace Grandes_Amigos.Models;

public class LoginViewInscrito
{
    [Required(ErrorMessage = "Debe ingresar su número de documento.")]
    public int NumDocumento { get; set; }

    [Required(ErrorMessage = "Debe ingresar su clave.")]
    public string? Clave { get; set; }
}
