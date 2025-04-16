using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grandes_Amigos.Models;

public class Usuario {
    [Key]
    public int ID {get;}
    public string NombreUsuario {get; set;}
    public string Clave {get; set;}
    [ForeignKey("IdMinisterio")]
    public int IdMinisterio {get; set;}

    public Usuario (int id, string nombre, string clave, int ministerio) {
        ID = id;
        NombreUsuario = nombre;
        Clave = clave;
        IdMinisterio = ministerio;
    }
}