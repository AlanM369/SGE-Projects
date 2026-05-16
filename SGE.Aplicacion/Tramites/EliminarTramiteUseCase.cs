// SGE.Aplicacion/Tramites/EliminarTramiteUseCase.cs

using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Tramites;


public class EliminarTramiteUseCase
{
    private readonly ITramiteRepository _tramiteRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly ActualizacionEstadoExpedienteService _actualizacionEstado;

    public EliminarTramiteUseCase(
        ITramiteRepository tramiteRepository,
        IAutorizacionService autorizacionService,
        ActualizacionEstadoExpedienteService actualizacionEstado)
    {
        _tramiteRepository = tramiteRepository;
        _autorizacionService = autorizacionService;
        _actualizacionEstado = actualizacionEstado;
    }

    public void Ejecutar(EliminarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
            throw new AutorizacionException("Sin permiso para eliminar trámites.");

        // Necesita el ExpedienteId antes de eliminar, porque después ya no existe
        var tramite = _tramiteRepository.ObtenerPorId(request.IdTramite)
            ?? throw new EntidadNoEncontradaException("Trámite no encontrado.");

        var expedienteId = tramite.ExpedienteId;

        _tramiteRepository.Eliminar(request.IdTramite);

        // Después de eliminar, recalcula el estado del expediente
        _actualizacionEstado.Ejecutar(expedienteId, request.IdUsuario);
    }
}