using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Autorizacion;

//Chequea los permisos
namespace SGE.Infraestructura;

public class AutorizacionService(IUsuarioRepository usuarioRepositorio) : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        //Busca al usuario en la base de datos
        var usuario = usuarioRepositorio.ObtenerPorId(idUsuario);

        if (usuario == null)
            return false;

        // Los administradores tienen todos los permisos
        if (usuario.EsAdministrador)
            return true;

        // Regla de implicancia (Lo pide el tp en 3.3): ExpedienteBaja implica TramiteBaja
        if (permiso == Permiso.TramiteBaja && usuario.Permisos.Contains(Permiso.ExpedienteBaja))
            return true;

        //Busca si tiene el permiso que necesita
        return usuario.Permisos.Contains(permiso);
    }
}