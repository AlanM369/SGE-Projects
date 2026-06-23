using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Usuarios;
using SGE.Infraestructura;

namespace SGE.WebApi;

/// <summary>
/// Inicializa la base de datos SQLite y carga los datos semilla (seed).
/// Se invoca una única vez al arrancar la aplicación.
/// </summary>
public static class DbSeedExtensions
{
    public static void InicializarBaseDeDatos(this WebApplication app)
    {
        // Creamos un scope para poder resolver servicios Scoped
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
        var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();

        // EnsureCreated: Crea el esquema si no existe aún.
        // Devuelve true si la BD se acaba de crear, false si ya existía.
        if (context.Database.EnsureCreated())
        {
            // ─── Configuración de journal_mode=DELETE (recomendación del TP2) ───
            // Esto asegura que los cambios se vean reflejados inmediatamente
            // en el archivo .sqlite sin necesidad de archivos WAL adicionales.
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode=DELETE;";
                command.ExecuteNonQuery();
            }

            // ─── Datos Semilla ───
            SeedUsuarios(context, hashService);
        }
    }

    private static void SeedUsuarios(SgeContext context, IHashService hashService)
    {
        // 1. Administrador Semilla (requerido por el TP2)
        var admin = Usuario.Reconstruir(
            id: Guid.NewGuid(),
            nombre: "Administrador",
            correo: "admin@sge.com",
            contrasenaHash: hashService.Hash("admin123"),
            esAdministrador: true
        );

        // 2. Usuario de prueba con permisos parciales
        //    Tiene permisos de alta y modificación de expedientes, pero no de baja
        var usuarioConPermisos = Usuario.Reconstruir(
            id: Guid.NewGuid(),
            nombre: "Juan Pérez",
            correo: "juan@sge.com",
            contrasenaHash: hashService.Hash("juan123"),
            esAdministrador: false
        );
        usuarioConPermisos.AgregarPermiso(Permiso.ExpedienteAlta);
        usuarioConPermisos.AgregarPermiso(Permiso.ExpedienteModificacion);
        usuarioConPermisos.AgregarPermiso(Permiso.TramiteAlta);
        usuarioConPermisos.AgregarPermiso(Permiso.TramiteModificacion);

        // 3. Usuario de prueba sin permisos (solo lectura)
        //    Solo puede consultar; no puede crear ni modificar nada
        var usuarioSinPermisos = Usuario.Reconstruir(
            id: Guid.NewGuid(),
            nombre: "María García",
            correo: "maria@sge.com",
            contrasenaHash: hashService.Hash("maria123"),
            esAdministrador: false
        );

        context.Usuarios.AddRange(admin, usuarioConPermisos, usuarioSinPermisos);
        context.SaveChanges();
    }
}
