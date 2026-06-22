// using Microsoft.EntityFrameworkCore;
// using SGE.Dominio.Expedientes;
// using SGE.Dominio.Tramites;
// using SGE.Dominio.Usuarios;


// //Como esta en la teoria aunque no sea lo mas optimo despues para hacer la webapi
// namespace SGE.Infraestructura;

// public class SgeContext : DbContext
// {
//     #nullable disable
//     public DbSet<Expediente> Expedientes { get; set; }
//     public DbSet<Tramite> Tramites { get; set; }
//     public DbSet<Usuario> Usuarios { get; set; }
//     #nullable restore

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlite("data source=SGE.sqlite");
//     }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);

//         // Value Objects mapeados con ComplexProperty
//         modelBuilder.Entity<Expediente>().ComplexProperty(e => e.Caratula);
//         modelBuilder.Entity<Tramite>().ComplexProperty(t => t.Contenido);
//     }
// }



//Forma correcta para el usuarioRepository actual
using Microsoft.EntityFrameworkCore;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura;

public class SgeContext : DbContext
{
    #nullable disable
    public DbSet<Expediente> Expedientes { get; set; }
    public DbSet<Tramite> Tramites { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    #nullable restore

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("data source=SGE.sqlite");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Value Objects mapeados con ComplexProperty
        modelBuilder.Entity<Expediente>().ComplexProperty(e => e.Caratula);
        modelBuilder.Entity<Tramite>().ComplexProperty(t => t.Contenido);

        // // La lista de permisos se serializa como string separado por comas al guardar, y se deserializa al leer
        // modelBuilder.Entity<Usuario>()
        //     .Property(u => u.Permisos)
        //     .HasConversion(
        //         permisos => string.Join(",", permisos.Select(p => p.ToString())),
        //         valor => string.IsNullOrWhiteSpace(valor)
        //             ? new List<Permiso>()
        //             : valor.Split(",", StringSplitOptions.RemoveEmptyEntries)
        //                    .Select(p => Enum.Parse<Permiso>(p))
        //                    .ToList()
        //     );

        //Los permisos son solo readonly, en teoria asi se puede mapear, terminar de chequear
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