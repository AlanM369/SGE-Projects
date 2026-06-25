using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace SGE.Infraestructura.Persistencia;

// Clase estática encargada de la creación inicial de la base de datos, la configuración del motor SQLite y la inserción de los datos semilla requeridos.
public static class SgeContextSeed
{
    public static void InicializarBaseDeDatos(SgeContext context, IHashService hashService, IUnidadDeTrabajo uow)
    {
        // 1. Verificación y creación de la base de datos si no existe previamente
        if (context.Database.EnsureCreated())
        {
            // 2. Apertura de la conexión física para ejecutar comandos nativos de SQLite
            var connection = context.Database.GetDbConnection();
            connection.Open();

            // 3. Configuración del modo de registro de transacciones en DELETE para asegurar escritura directa e inmediata
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode=DELETE;";
                command.ExecuteNonQuery();
            }

            // 4. Invocación al método encargado de poblar las tablas con los usuarios iniciales
            SeedUsuarios(context, hashService, uow);
        }
    }

    private static void SeedUsuarios(SgeContext context, IHashService hashService, IUnidadDeTrabajo uow)
    {
        // 1. Administrador Semilla
        var admin = Usuario.Reconstruir(
            id: Guid.NewGuid(),
            nombre: "Administrador",
            correo: "admin@sge.com",
            contrasenaHash: hashService.Hash("admin123"),
            esAdministrador: true
        );

        // 2. Usuario con permisos parciales
        var usuarioConPermisos = new Usuario(
            "Juan Pérez",
            "juan@sge.com",
            hashService.Hash("juan123"));

        usuarioConPermisos.AgregarPermiso(Permiso.ExpedienteAlta);
        usuarioConPermisos.AgregarPermiso(Permiso.ExpedienteModificacion);
        usuarioConPermisos.AgregarPermiso(Permiso.TramiteAlta);
        usuarioConPermisos.AgregarPermiso(Permiso.TramiteModificacion);

        // 3. Usuario sin permisos (solo lectura)
        var usuarioSinPermisos = new Usuario(
            "María García",
            "maria@sge.com",
            hashService.Hash("maria123"));

        context.Usuarios.Add(admin);
        context.Usuarios.Add(usuarioConPermisos);
        context.Usuarios.Add(usuarioSinPermisos);
        uow.Guardar();
    }
}