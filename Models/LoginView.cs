using System.ComponentModel.DataAnnotations;
namespace Grandes_Amigos.Models;

public class LoginView {
	[Required (ErrorMessage = "Es necesario el nombre de usuario")]
	public string NombreUsuario {get; set;}

	[DataType(DataType.Password)]
	[Required (ErrorMessage = "Es necesaria la contraseña")]
	public string Clave {get; set;}
	public LoginView () {

	}
}