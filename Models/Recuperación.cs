using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Table("Recuperación")]
public class Recuperación
{
    [Key]
    [Required]
    public string Token_Recuperación { get; set; }

    [Required]
    public int ID_Usuario { get; set; }

    [Required]
    public DateTime Válido_Hasta { get; set; }

    public Recuperación () {}
}