using SGE.Aplicacion.Autorizacion; // Para que reconozca el enumerativo Permiso

namespace SGE.Infraestructura;

// La clase implementa el contrato definido en la capa de Aplicación
public class AutorizacionProvisionalService : IAutorizacionService
{
    // El método requerido por la interfaz
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        // Para esta entrega provisional, el método devuelve siempre true
        // Esto evita trabar el desarrollo de los Casos de Uso
        return false;
    }
}