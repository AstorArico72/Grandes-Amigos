using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Table("Inscripciones")]
public class Inscripción {
    // Al parecer, no es posible trabajar con EF Core sin claves primarias. Fuente: https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types
    [Key]
    [Column("ID")]
    public int ID_Inscripción {get; set;}

    [ForeignKey("ID_Evento")]
    public int ID_Evento {get; set;}
    [ForeignKey("ID_Inscrito")]

    public int ID_Inscrito {get; set;}

    
    //public Evento Evento { get; set; }

    //public Inscrito Inscrito { get; set; }

    //Constructor requerido por EF Core
    public Inscripción() { }

    // Constructor personalizado si lo necesitás en lógica de negocio
    public Inscripción(int idInscripcion, int idEvento, int idInscrito) {
        ID_Inscripción = idInscripcion;
        ID_Evento = idEvento;
        ID_Inscrito = idInscrito;
    }
    public Inscripción(int idEvento, int idInscrito) {
        ID_Evento = idEvento;
        ID_Inscrito = idInscrito;
    }
}
