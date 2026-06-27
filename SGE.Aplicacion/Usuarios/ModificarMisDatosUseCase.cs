using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarMisDatosUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow, IHashService hashService)
{
    public ModificarMisDatosResponse Ejecutar(ModificarMisDatosRequest request)
    {
        var usuario = repositorio.ObtenerPorId(request.IdUsuario)
            ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuario}.");

        usuario.ModificarDatos(request.NuevoNombre, request.NuevoCorreo);

        if (!string.IsNullOrWhiteSpace(request.NuevaContrasena))
        {
            var nuevoHash = hashService.Hash(request.NuevaContrasena);
            usuario.CambiarContrasena(nuevoHash);
        }

        repositorio.Modificar(usuario);
        uow.Guardar();

        return new ModificarMisDatosResponse();
    }
}