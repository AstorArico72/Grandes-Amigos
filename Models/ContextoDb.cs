using Grandes_Amigos.Models;
using Microsoft.EntityFrameworkCore;

public class ContextoDb : DbContext
{
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Inscripción> Inscripciones { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Ministerio> Ministerios { get; set; }
    public DbSet<Administrador> Admins { get; set; }
    public DbSet<Noticia> Noticias { get; set; }
    public DbSet<Recuperación> Tokens { get; set; }

    public ContextoDb(DbContextOptions<ContextoDb> opciones)
        : base(opciones) { }
}
