using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarMisDatosUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow, IHashService hashService)
{
    public void Ejecutar(ModificarMisDatosRequest request)
    {
        // 1. Buscamos al usuario
        var usuario = repositorio.ObtenerPorId(request.IdUsuario)
            ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuario}.");

        // 2. Actualizamos los datos personales
        usuario.ModificarDatos(request.NuevoNombre, request.NuevoCorreo);

        // 3. Si mandó una nueva contraseña, la actualizamos
        if (!string.IsNullOrWhiteSpace(request.NuevaContrasena))
        {
            var nuevoHash = hashService.Hash(request.NuevaContrasena);
            usuario.CambiarContrasena(nuevoHash);
        }

        repositorio.Modificar(usuario);
        uow.Guardar();
    }
}