using System.ComponentModel.DataAnnotations.Schema;
using Grandes_Amigos.Models;
using Microsoft.EntityFrameworkCore;
public class ContextoDb : DbContext {
    private readonly string ConnectionString;

    public DbSet<Evento> Eventos;
    public DbSet<Inscripción> Inscripciones;
    public DbSet<Usuario> Usuarios;
    public DbSet<Ministerio> Ministerios;
    public DbSet<Inscrito> Inscritos;

    public ContextoDb (DbContextOptions<ContextoDb> opciones) : base (opciones) {

    }
}