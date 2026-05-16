// ITramiteRepository establece qué operaciones existen sobre los trámites, sin importar cómo se implementan.
// La capa de Aplicación solo conoce esta interfaz, no su implementacion, que luego la capa de infraestructura 
// se encargará de sobreescribir, permitiendo así la separación de responsabilidades.

using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;


public interface ITramiteRepository
{
    void Agregar(Tramite tramite);
    Tramite? ObtenerPorId(Guid id);
    IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId);
    void Modificar(Tramite tramite);
    void Eliminar(Guid id);
    void EliminarPorExpedienteId(Guid expedienteId);
}