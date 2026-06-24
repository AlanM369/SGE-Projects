using Microsoft.EntityFrameworkCore;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura;

public class SgeContext : DbContext
{
    // Constructor para inyección de dependencias (lo usa la WebApi)
    public SgeContext(DbContextOptions<SgeContext> options) : base(options) { }

    // Constructor sin parámetros (fallback para la Consola)
    public SgeContext() { }

    #nullable disable
    public DbSet<Expediente> Expedientes { get; set; }
    public DbSet<Tramite> Tramites { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    #nullable restore

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("data source=SGE.sqlite");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Expediente>().ComplexProperty(e => e.Caratula, b =>
    {
        b.Property(c => c.Texto);
    });

    modelBuilder.Entity<Tramite>().ComplexProperty(t => t.Contenido, b =>
    {
        b.Property(c => c.Texto);
    });modelBuilder.Entity<Expediente>().ComplexProperty(e => e.Caratula, b =>
    {
        b.Property(c => c.Texto);
    });

    modelBuilder.Entity<Tramite>().ComplexProperty(t => t.Contenido, b =>
    {
        b.Property(c => c.Texto);
    });

        modelBuilder.Entity<Usuario>(b =>
        {
            b.Property<List<Permiso>>("_permisos")
            .HasColumnName("Permisos")
            .HasField("_permisos")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                permisos => string.Join(",", permisos.Select(p => p.ToString())),
                valor => string.IsNullOrWhiteSpace(valor)
                    ? new List<Permiso>()
                    : valor.Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => Enum.Parse<Permiso>(p))
                            .ToList()
            );
        });
    }
}