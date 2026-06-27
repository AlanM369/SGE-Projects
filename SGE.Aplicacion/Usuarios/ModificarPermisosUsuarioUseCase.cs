using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow)
{
    public ModificarPermisosResponse Ejecutar(ModificarPermisosRequest request)
    {
        var admin = repositorio.ObtenerPorId(request.IdAdministrador)
            ?? throw new EntidadNoEncontradaException("Administrador no encontrado.");

        if (!admin.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede modificar permisos.");

        var usuario = repositorio.ObtenerPorId(request.IdUsuarioObjetivo)
            ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuarioObjetivo}.");

        usuario.ReemplazarPermisos(request.NuevosPermisos);

        repositorio.Modificar(usuario);
        uow.Guardar();

        return new ModificarPermisosResponse();
    }
}