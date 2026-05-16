using SGE.Aplicacion.Autorizacion;

namespace SGE.Infraestructura;

// Siempre devuelve true.
public class AutorizacionProvisionalService : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        return true;
    }
}