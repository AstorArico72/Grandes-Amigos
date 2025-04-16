using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Grandes_Amigos.Models;

public class Usuario {
    [Key]
    public int ID {get;}
    // [Index (IsUnique =true)] no parece servir aquí, hay que marcar el nombre de usuario como único de alguna forma.
    public string NombreUsuario {get; set;}
    public string Clave {get; set;}
    [ForeignKey("Usuario-Ministerio")]
    public int IdMinisterio {get; set;}

    public Usuario (int id, string nombre, string clave, int ministerio) {
        ID = id;
        NombreUsuario = nombre;
        Clave = clave;
        IdMinisterio = ministerio;
    }
}