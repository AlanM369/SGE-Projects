namespace SGE.Aplicacion.Tramites;

public class ListarTramitesPorExpedienteUseCase(ITramiteRepository tramiteRepositorio)
{
    // Le pasamos el ID del expediente por parámetro
    public IEnumerable<TramiteDetalleDTO> Ejecutar(ListarTramitesPorExpedienteRequest request)
    {
        var tramites = tramiteRepositorio.ObtenerPorExpedienteId(request.ExpedienteId);
        var dtos = new List<TramiteDetalleDTO>();

        foreach (var t in tramites)
        {   // Mapeamos de Entidad a DTO para proteger el encapsulamiento
            // Extraemos el texto del Value Objec
            var dto = new TramiteDetalleDTO(t.Id, t.ExpedienteId, t.Etiqueta, t.Contenido.Texto, t.FechaCreacion, t.FechaUltimaModificacion, t.UsuarioUltimoCambio);
            dtos.Add(dto);
        }
        return dtos.OrderBy(t => t.FechaCreacion).ToList();
    }
}