using System.ComponentModel.DataAnnotations;

namespace Grandes_Amigos.Models.ViewModels
{
    public class RegistroUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        public string TipoDocumento { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [Range(1, 999999999, ErrorMessage = "El DNI debe ser un número válido.")]
        public int? NumDocumento { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 dígitos.")]
        public string Teléfono { get; set; }

        [Required(ErrorMessage = "La asociación es obligatoria.")]
        public string Asociación { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "La clave debe tener al menos 6 caracteres."
        )]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Debes repetir la clave.")]
        [Compare("Clave", ErrorMessage = "Las claves no coinciden.")]
        public string RepetirClave { get; set; }
    }
}
