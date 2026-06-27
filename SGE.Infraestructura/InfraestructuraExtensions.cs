using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;
using SGE.Infraestructura.Repositorios;
using SGE.Infraestructura.Persistencia;
using SGE.Infraestructura.Seguridad;

namespace SGE.Infraestructura;

// Centralizar la configuración e inyección de dependencias de la capa de infraestructura en el contenedor de .NET.
public static class InfraestructuraExtensions
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        //1. Configuración de JwtSettings desde appsettings.json
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("La sección 'JwtSettings' no está configurada en appsettings.json.");

        services.AddSingleton(jwtSettings);

        //2. EF Core con SQLite
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=SGE.sqlite";

        services.AddDbContext<SgeContext>(options =>
            options.UseSqlite(connectionString));

        //3. Unidad de Trabajo
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        //4. Repositorios
        services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
        services.AddScoped<ITramiteRepository, TramiteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        //5. Servicios de Infraestructura
        services.AddScoped<IAutorizacionService, AutorizacionService>();
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<ITokenProvider, JwtTokenProvider>();

        return services;
    }
}