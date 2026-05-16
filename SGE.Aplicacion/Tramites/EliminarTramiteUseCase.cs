// SGE.Aplicacion/Tramites/EliminarTramiteUseCase.cs

using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Tramites;


public class EliminarTramiteUseCase
{
    // Dependencias necesarias para eliminar un trámite y actualizar el estado del expediente. Solo de lectura porque no van a ser modificadas.
    private readonly ITramiteRepository _tramiteRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly ActualizacionEstadoExpedienteService _actualizacionEstado;

    // Constructor que recibe las dependencias
    public EliminarTramiteUseCase(
        ITramiteRepository tramiteRepository,
        IAutorizacionService autorizacionService,
        ActualizacionEstadoExpedienteService actualizacionEstado)
    {
        _tramiteRepository = tramiteRepository;
        _autorizacionService = autorizacionService;
        _actualizacionEstado = actualizacionEstado;
    }

    // Método principal que ejecuta el caso de uso. Recibe un DTO request con los datos necesarios para eliminar el trámite. 
    // Si el usuario no tiene permiso, o el trámite no existe, lanza excepciones.
    public void Ejecutar(EliminarTramiteRequest request)
    {
        // Verificamos que el usuario tenga permiso para eliminar trámites. Si no lo tiene, lanza una excepción de autorización.
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
            throw new AutorizacionException("Sin permiso para eliminar trámites.");

        // Si tiene permiso, busca el trámite a eliminar. Si no existe, lanza una excepción de entidad no encontrada.
        var tramite = _tramiteRepository.ObtenerPorId(request.IdTramite)
            ?? throw new EntidadNoEncontradaException("Trámite no encontrado.");

        var expedienteId = tramite.ExpedienteId; // Guardamos el id del expediente antes de eliminar el trámite, para poder actualizar su estado después.

        _tramiteRepository.Eliminar(request.IdTramite); // Eliminamos el trámite usando el repositorio.

        // Después de eliminar, actualizamos el estado del expediente al que pertenece el trámite.
        _actualizacionEstado.Ejecutar(expedienteId, request.IdUsuario);
    }
}