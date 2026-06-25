using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;


namespace SGE.Aplicacion.Expedientes;

//Busca un expediente y lo agrupa con sus tramites antes de devolver un DTO
//Como es de lectura no necesita chequear los permisos
public class ObtenerExpedienteConTramitesUseCase(IExpedienteRepository expedienteRepositorio, ITramiteRepository tramiteRepositorio)
{
    public ExpedienteConTramitesDTO Ejecutar(ObtenerExpedienteConTramitesRequest request)
    {
        // 1. Se busca el expediente, si no se encuentra lanza excpecion
        var expediente = expedienteRepositorio.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}.");

        // 2. Busca los trámites del expediente con el .Select
        var tramites = tramiteRepositorio.ObtenerPorExpedienteId(request.ExpedienteId)
            .Select(t => new TramiteDetalleDTO(t.Id, t.ExpedienteId, t.Etiqueta, t.Contenido.Texto, t.FechaCreacion, t.FechaUltimaModificacion, t.UsuarioUltimoCambio));

        // 3. Devuelve un DTO con los datos del expediente y la lista de trámites
        return new ExpedienteConTramitesDTO(
            expediente.Id,
            expediente.Caratula.Texto,
            expediente.Estado,
            expediente.FechaCreacion,
            expediente.FechaUltimaModificacion,
            tramites.ToList()
        );
    }
}