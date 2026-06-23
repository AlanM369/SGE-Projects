using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Usuarios;
using SGE.Infraestructura;
using SGE.Infraestructura.Repositorios;

namespace SGE.WebApi;

/// <summary>
/// Métodos de extensión para registrar los servicios de infraestructura.
/// Siguiendo la sugerencia del TP2 de mantener un Program.cs limpio.
/// </summary>
public static class InfraestructuraExtensions
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        // ─── 1. Configuración de JwtSettings desde appsettings.json ───
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("La sección 'JwtSettings' no está configurada en appsettings.json.");

        services.AddSingleton(jwtSettings);

        // ─── 2. EF Core con SQLite ───
        // Usamos la cadena de conexión del appsettings para indicar la ruta de la BD
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=SGE.sqlite";

        services.AddDbContext<SgeContext>(options =>
            options.UseSqlite(connectionString));

        // ─── 3. Unidad de Trabajo ───
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        // ─── 4. Repositorios (Scoped: un ciclo de vida por petición HTTP) ───
        services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
        services.AddScoped<ITramiteRepository, TramiteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // ─── 5. Servicios de Infraestructura ───
        services.AddScoped<IAutorizacionService, AutorizacionService>();
        services.AddScoped<IHashService, HashService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
