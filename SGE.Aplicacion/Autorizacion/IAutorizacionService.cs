namespace SGE.Aplicacion.Autorizacion;

// Define si un usuario tiene un permiso determinado
public interface IAutorizacionService
{
    // Recibe el id del usuario que intenta realizar la acción y el permiso requerido.
    // Devuelve true si el usuario tiene el permiso, false si no lo tiene
    bool PoseeElPermiso(Guid idUsuario, Permiso permiso);
}