using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Recuperación")]
public class Recuperación
{
    [Key]
    [Required]
    [Column("Token_Recuperación")]
    public string Token_Recuperación { get; set; }

    // Puede ser token para un usuario o para un admin -> nullable
    [Column("ID_Usuario")]
    public int? ID_Usuario { get; set; }

    // Nuevo: si el token pertenece a un administrador
    [Column("ID_Admin")]
    public int? ID_Admin { get; set; }

    [Required]
    [Column("Válido_Hasta")]
    public DateTime Válido_Hasta { get; set; }

    // Campo que ya existe en la tabla dump; opcional pero útil
    [Column("Rol")]
    public string Rol { get; set; }

    public Recuperación() { }
}
