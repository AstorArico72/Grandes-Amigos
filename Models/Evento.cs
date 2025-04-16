using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

[Table("Eventos")]
public class Evento {
    [Key]
    public int ID {get;}
    public DateTime Fecha {get; set;}
    public string Título {get; set;}
    public string Descripción {get; set;}
    [ForeignKey("Ministerio-Evento")]
    public int IdMinisterio {get; set;}

    public Evento (string titulo, string descripcion, DateTime fecha, int ministerio) {
        Título = titulo;
        Descripción = descripcion;
        Fecha = fecha;
        IdMinisterio = ministerio;
    }
}