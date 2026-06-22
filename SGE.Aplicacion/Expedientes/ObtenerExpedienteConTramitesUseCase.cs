using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;


namespace SGE.Aplicacion.Expedientes;

//Busca un expediente y lo agrupa con sus tramites antes de devolver un DTO
//Como es de lectura no necesita chequear los permisos
public class ObtenerExpedienteConTramitesUseCase(IExpedienteRepository expedienteRepositorio, ITramiteRepository tramiteRepositorio)
{
    public ExpedienteConTramitesDTO Ejecutar(Guid expedienteId)
    {
        //Se bsuca el expediente, si no se encuentra lanza excpecion
        var expediente = expedienteRepositorio.ObtenerPorId(expedienteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {expedienteId}.");

        //Busca los tramites del expediente con el .Select
        var tramites = tramiteRepositorio.ObtenerPorExpedienteId(expedienteId)
            .Select(t => new TramiteDetalleDTO(t.Id, t.ExpedienteId, t.Etiqueta, t.Contenido.Texto, t.FechaCreacion, t.FechaUltimaModificacion, t.UsuarioUltimoCambio));

        //DTO con los datos del expediente y al lista de tramites
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