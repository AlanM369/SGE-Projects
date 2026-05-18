using SGE.Aplicacion.Autorizacion; // Para que reconozca el enumerativo Permiso

namespace SGE.Infraestructura;

// La clase implementa el contrato definido en la capa de Aplicación
public class AutorizacionProvisionalService : IAutorizacionService
{   
    // Propiedad pública configurable
    public bool Autorizar { get; set; } = true;
    
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        return Autorizar;
    }
}