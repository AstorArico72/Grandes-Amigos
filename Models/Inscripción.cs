using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

[Table("Inscripciones")]
public class Inscripción
{
    [Key]
    public int ID { get; set; }

    public int ID_Evento { get; set; }

    public int ID_Inscrito { get; set; }

    [ForeignKey("ID_Evento")]
    public Evento Evento { get; set; }

    [ForeignKey("ID_Inscrito")]
    public Inscrito Inscrito { get; set; }

    // ✅ Constructor requerido por EF Core
    public Inscripción() { }

    // Constructor personalizado si lo necesitás en lógica de negocio
    public Inscripción(int idEvento, int idInscrito)
    {
        ID_Evento = idEvento;
        ID_Inscrito = idInscrito;
    }
}
