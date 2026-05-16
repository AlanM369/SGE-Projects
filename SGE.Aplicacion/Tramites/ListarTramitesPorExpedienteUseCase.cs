using SGE.Aplicacion.Interfaces;

namespace SGE.Aplicacion.Tramites;

public class ListarTramitesPorExpedienteUseCase
{
    private readonly ITramiteRepository _tramiteRepositorio;

    public ListarTramitesPorExpedienteUseCase(ITramiteRepository tramiteRepositorio)
    {
        _tramiteRepositorio = tramiteRepositorio;
    }

    // Le pasamos el ID del expediente por parámetro
    public IEnumerable<TramiteDetalleDTO> Ejecutar(Guid expedienteId)
    {
        var tramites = _tramiteRepositorio.ObtenerPorExpedienteId(expedienteId);
        var listaDinamica = new List<TramiteDetalleDTO>();

        foreach (var t in tramites)
        {   // Mapeamos de Entidad a DTO para proteger el encapsulamiento
            var dto = new TramiteDetalleDTO(
                t.Id, 
                t.ExpedienteId, 
                t.Etiqueta, 
                t.Contenido.Texto, // Extraemos el texto del Value Object
                t.FechaCreacion, 
                t.FechaUltimaModificacion
            );

            listaDinamica.Add(dto);
        }
        return listaDinamica;
    }
}