using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Autorizacion;

// Este contrato nos va a permitir preguntar si un usuario puede hacer una acción,
// aislando la lógica de seguridad del Caso de Uso.
public interface IAutorizacionService
{   
    // Recibe el id del usuario que intenta realizar la acción y el permiso requerido.
    // Devuelve true si el usuario tiene el permiso, false si no lo tiene
    bool PoseeElPermiso(Guid idUsuario, Permiso permiso);
}