using SGE.Aplicacion.Comun;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public class RegistrarUsuarioUseCase(IUsuarioRepository repositorio, IUnidadDeTrabajo uow, IHashService hashService)
{
    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        // 1. Verificamos que el correo no este registrado
        var existente = repositorio.ObtenerPorCorreo(request.CorreoElectronico);
        if (existente != null)
            throw new EntidadDuplicadaException($"El correo '{request.CorreoElectronico}' ya está registrado.");

        // 2. Hasheamos la contraseña antes de pasarla al dominio
        var hash = hashService.Hash(request.Contrasena);

        // 3. Creamos la entidad de dominio
        var usuario = new Usuario(request.Nombre, request.CorreoElectronico, hash);

        // 4. Persistencia
        repositorio.Agregar(usuario);
        uow.Guardar();

        return new RegistrarUsuarioResponse(usuario.Id);
    }
}