using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class EliminarUsuarioUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow)
{
    public void Ejecutar(EliminarUsuarioRequest request)
    {
        // 1. Verificamos que quien llama sea administrador
        var admin = repositorio.ObtenerPorId(request.IdAdministrador)
            ?? throw new EntidadNoEncontradaException("Administrador no encontrado.");

        if (!admin.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede eliminar usuarios.");

        // 2. Verificamos que el usuario a eliminar exista
        var usuario = repositorio.ObtenerPorId(request.IdUsuarioAEliminar)
            ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuarioAEliminar}.");

        repositorio.Eliminar(usuario.Id);
        uow.Guardar();
    }
}