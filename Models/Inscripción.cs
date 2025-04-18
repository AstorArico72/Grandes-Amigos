using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Keyless]
public class Inscripción {
    [ForeignKey("Evento-Inscripción")]
    public int IdEvento {get;}
    [ForeignKey("Inscrito-Inscripción")]
    public int IdInscrito {get;}

    public Inscripción (int evento, int inscrito) {
        IdEvento = evento;
        IdInscrito = inscrito;
    }
}