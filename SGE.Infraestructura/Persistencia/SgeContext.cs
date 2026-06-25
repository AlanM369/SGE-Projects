using Microsoft.EntityFrameworkCore;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Persistencia;

// Mapear las entidades de dominio hacia las tablas correspondientes en la base de datos
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

        // Mapeo de los Objetos de Valor mediante propiedades complejas
        modelBuilder.Entity<Expediente>().ComplexProperty(e => e.Caratula, b =>
        {
            b.Property(c => c.Texto);
        });

        modelBuilder.Entity<Tramite>().ComplexProperty(t => t.Contenido, b =>
        {
            b.Property(c => c.Texto);
        });

        // Configuración avanzada de mapeo para la entidad Usuario
        modelBuilder.Entity<Usuario>(b =>
        {
            // Definición del comparador personalizado para detectar correctamente cambios en colecciones mutables
            var comparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Permiso>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            // Configuración del acceso directo al campo privado y conversión de la lista de permisos a texto plano
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
            )
            .Metadata.SetValueComparer(comparer);
        });
    }
}