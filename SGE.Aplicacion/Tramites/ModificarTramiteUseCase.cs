// SGE.Aplicacion/Tramites/ModificarTramiteUseCase.cs

using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;
namespace SGE.Aplicacion.Tramites;


public class ModificarTramiteUseCase
{
    // Dependencias necesarias para agregar un trámite y actualizar el estado del expediente. Solo de lectura porque no van a ser modificadas.
    private readonly ITramiteRepository _tramiteRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly ActualizacionEstadoExpedienteService _actualizacionEstado;

    // Constructor que recibe las dependencias
    public ModificarTramiteUseCase(
        ITramiteRepository tramiteRepository,
        IAutorizacionService autorizacionService,
        ActualizacionEstadoExpedienteService actualizacionEstado)
    {
        _tramiteRepository = tramiteRepository;
        _autorizacionService = autorizacionService;
        _actualizacionEstado = actualizacionEstado;
    }

    // Método principal que ejecuta el caso de uso. Recibe un DTO request con los datos necesarios para modificar el trámite.
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        // Verifica que el usuario tenga permiso para modificar trámites. Si no lo tiene, lanza una excepción de autorización.
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteModificacion))
            throw new AutorizacionException("Sin permiso para modificar trámites.");

        // Si tiene permiso, busca el trámite a modificar. Si no existe, lanza una excepción de entidad no encontrada.
        var tramite = _tramiteRepository.ObtenerPorId(request.IdTramite)
            ?? throw new EntidadNoEncontradaException("Trámite no encontrado.");

        // Modifica el trámite con los nuevos datos del request. El método ModificarTramite se encarga de validar los datos y lanzar excepciones si son inválidos.
        var nuevoContenido = new ContenidoTramite(request.NuevoContenido);
        tramite.ModificarTramite(request.NuevaEtiqueta, nuevoContenido, request.IdUsuario);

        _tramiteRepository.Modificar(tramite); // Se guarda el trámite modificado usando el repositorio.

        _actualizacionEstado.Ejecutar(tramite.ExpedienteId, request.IdUsuario); // Después de modificar, actualiza el estado del expediente al que pertenece el trámite.

        // Por ultimo devuelve un DTO response con el id del trámite modificado, la nueva etiqueta y el nuevo contenido del trámite.
        return new ModificarTramiteResponse(tramite.Id, tramite.Etiqueta, tramite.Contenido!.Texto);
    }
}