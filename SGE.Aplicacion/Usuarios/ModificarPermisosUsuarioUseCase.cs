using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow)
{
    public void Ejecutar(ModificarPermisosRequest request)
    {
        // 1. Verificamos que quien llama sea administrador
        var admin = repositorio.ObtenerPorId(request.IdAdministrador)
            ?? throw new EntidadNoEncontradaException("Administrador no encontrado.");

        if (!admin.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede modificar permisos.");

        // 2. Buscamos al usuario objetivo
        var usuario = repositorio.ObtenerPorId(request.IdUsuarioObjetivo)
            ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuarioObjetivo}.");

        // 3. Reemplazamos los permisos usando el método del dominio
        usuario.ReemplazarPermisos(request.NuevosPermisos);

        repositorio.Modificar(usuario);
        uow.Guardar();
    }
}