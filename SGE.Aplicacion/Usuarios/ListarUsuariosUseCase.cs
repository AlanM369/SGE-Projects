using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ListarUsuariosUseCase(IUsuarioRepository repositorio)
{
    public IEnumerable<UsuarioDetalleDTO> Ejecutar(Guid idAdministrador)
    {
        // 1. Verificamos que quien llama sea administrador
        var admin = repositorio.ObtenerPorId(idAdministrador)
            ?? throw new EntidadNoEncontradaException("Administrador no encontrado.");

        if (!admin.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede listar usuarios.");

        // 2. Obtenemos y mapeamos
        return repositorio.ObtenerTodos()
            .Select(u => new UsuarioDetalleDTO(u.Id, u.Nombre, u.CorreoElectronico, u.EsAdministrador, u.Permisos));
    }
}