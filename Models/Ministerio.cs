using System.ComponentModel.DataAnnotations;

namespace Grandes_Amigos.Models;

public class Ministerio {
    [Key]
    public int ID {get;}
    public string Nombre {get; set;}

    public Ministerio (int id, string nombre) {
        ID = id;
        Nombre = nombre;
    }
}