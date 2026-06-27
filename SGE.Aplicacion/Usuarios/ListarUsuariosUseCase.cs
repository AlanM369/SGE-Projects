using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ListarUsuariosUseCase(IUsuarioRepository repositorio)
{
    public IEnumerable<UsuarioDetalleDTO> Ejecutar(ListarUsuariosRequest request)
    {
        var admin = repositorio.ObtenerPorId(request.IdAdministrador)
            ?? throw new EntidadNoEncontradaException("Administrador no encontrado.");

        if (!admin.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede listar usuarios.");

        return repositorio.ObtenerTodos()
            .Select(u => new UsuarioDetalleDTO(u.Id, u.Nombre, u.CorreoElectronico, u.EsAdministrador, u.Permisos))
            .OrderBy(u => u.Nombre);
    }
}