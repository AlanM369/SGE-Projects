using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public interface IExpedienteRepository
{
    void Agregar(Expediente expediente);
    Expediente? ObtenerPorId(Guid id); //Si el ID no existe en el archivo, devuelve null
    IEnumerable<Expediente> ObtenerTodos(); // Devuelve todos los expedientes
    void Modificar(Expediente expediente);
    void Eliminar(Guid id);
}