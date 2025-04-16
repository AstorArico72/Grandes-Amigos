using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Keyless]
public class Inscripción {
    [ForeignKey("IdEvento")]
    public int IdEvento {get;}
    [ForeignKey("IdInscrito")]
    public int IdInscrito {get;}
}