using System.ComponentModel.DataAnnotations;
using Grandes_Amigos.Models;

namespace Grandes_Amigos.Models.ViewModels
{
    public class AdminListItemViewModel
    {
        public int ID { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MinisterioNombre { get; set; } = string.Empty;
        public int IdMinisterio { get; set; }
    }

    public class AdminFormViewModel
    {
        public int? ID { get; set; }

        [Required]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? Clave { get; set; }

        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las contrasenas deben coincidir.")]
        public string? ConfirmarClave { get; set; }

        [Display(Name = "Ministerio")]
        public int IdMinisterio { get; set; } = 1;

        public bool EsEdicion { get; set; }

        public IEnumerable<Ministerio>? Ministerios { get; set; }
    }
}
