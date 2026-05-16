// SGE.Aplicacion/Tramites/ListarTramitesPorExpedienteUseCase.cs

using SGE.Dominio.Tramites;
namespace SGE.Aplicacion.Tramites;

public class ListarTramitesPorExpedienteUseCase
{
    // Dependencia necesaria para obtener los trámites de un expediente. 
    private readonly ITramiteRepository _tramiteRepository;

    // Constructor que recibe la dependencia
    public ListarTramitesPorExpedienteUseCase(ITramiteRepository tramiteRepository)
    {
        _tramiteRepository = tramiteRepository;
    }

    // No necesita autorización ya que solo va a leer los tramites de un expediente. Si el expediente no existe, el repositorio devuelve una lista vacía.
    public IEnumerable<TramiteResponse> Ejecutar(Guid expedienteId)
    {
        return _tramiteRepository   
            .ObtenerPorExpedienteId(expedienteId) // Obtiene la lista de trámites del id del expediente usando el repositorio. 
            .Select(t => new TramiteResponse(
                t.Id,
                t.ExpedienteId,
                t.Etiqueta,
                t.Contenido!.Texto,
                t.FechaCreacion));
    }
}