using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Interfaces;

public interface IAutorizacionService
{
    bool PoseeElPermiso(Guid idUsuario, Permiso permiso);
}