using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;

namespace SGE.Aplicacion;

public static class AplicacionExtensions
{
    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        // ─── Casos de Uso de Expedientes ───
        services.AddScoped<AgregarExpedienteUseCase>();
        services.AddScoped<BajaExpedienteUseCase>();
        services.AddScoped<ModificarCaratulaExpedienteUseCase>();
        services.AddScoped<CambiarEstadoExpedienteUseCase>();
        services.AddScoped<ListarExpedientesUseCase>();
        services.AddScoped<ObtenerExpedienteConTramitesUseCase>();

        // ─── Casos de Uso de Trámites ───
        services.AddScoped<AgregarTramiteUseCase>();
        services.AddScoped<BajaTramiteUseCase>();
        services.AddScoped<ModificarTramiteUseCase>();
        services.AddScoped<ListarTramitesPorExpedienteUseCase>();

        // ─── Servicio de dominio: actualización automática del estado ───
        services.AddScoped<ActualizacionEstadoExpedienteService>();

        // ─── Casos de Uso de Usuarios / Autenticación ───
        services.AddScoped<RegistrarUsuarioUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<ModificarMisDatosUseCase>();
        services.AddScoped<ListarUsuariosUseCase>();
        services.AddScoped<EliminarUsuarioUseCase>();
        services.AddScoped<ModificarPermisosUsuarioUseCase>();

        return services;
    }
}