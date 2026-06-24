using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;
using SGE.Dominio.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace SGE.Infraestructura;

public static class SgeContextSeed
{
    public static void InicializarBaseDeDatos(SgeContext context, IHashService hashService, IUnidadDeTrabajo uow)
    {
        if (context.Database.EnsureCreated())
        {
            // ─── journal_mode=DELETE (recomendación del TP2) ───
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode=DELETE;";
                command.ExecuteNonQuery();
            }

            // ─── Datos Semilla ───
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