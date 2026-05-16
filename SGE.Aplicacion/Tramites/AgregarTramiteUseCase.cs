// SGE.Aplicacion/Tramites/AgregarTramiteUseCase.cs
using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class AgregarTramiteUseCase
{
    // Dependencias necesarias para agregar un trámite y actualizar el estado del expediente. Solo de lectura porque no van a ser modificadas.
    private readonly ITramiteRepository _tramiteRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly ActualizacionEstadoExpedienteService _actualizacionEstado;

    // Constructor que recibe las dependencias
    public AgregarTramiteUseCase(
        ITramiteRepository tramiteRepository,
        IAutorizacionService autorizacionService,
        ActualizacionEstadoExpedienteService actualizacionEstado)
    {
        _tramiteRepository = tramiteRepository;
        _autorizacionService = autorizacionService;
        _actualizacionEstado = actualizacionEstado;
    }

    // Método principal que ejecuta el caso de uso. Recibe un DTO request con los datos necesarios para agregar el trámite.
    // y devuelve un DTO response con el resultado. Si el usuario no tiene permiso, o el expediente no existe, lanza excepciones.
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        // Verifica que el usuario tenga permiso para agregar trámites. Si no lo tiene, lanza una excepción de autorización.
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteAlta))
            throw new AutorizacionException("Sin permiso para agregar trámites.");

        // Si tiene permiso, crea un nuevo trámite a partir de los datos del request. 
        var contenido = new ContenidoTramite(request.Contenido);
        var tramite = new Tramite(request.ExpedienteId, request.Etiqueta, contenido, request.IdUsuario); // El constructor del trámite se encarga de validar los datos y lanzar excepciones si son inválidos.

        _tramiteRepository.Agregar(tramite); // Se guarda el nuevo trámite usando el repositorio.

        // Como se agregó un trámite nuevo, el estado del expediente al que pertenece el tramite puede haber cambiado. 
        // El servicio se encarga de buscar el último trámite y decidir si el estado cambia o no
        _actualizacionEstado.Ejecutar(request.ExpedienteId, request.IdUsuario);

        // Por ultimo devuelve un DTO response con el id del nuevo trámite, el id del expediente al que pertenece y la etiqueta del trámite.
        return new AgregarTramiteResponse(tramite.Id, tramite.ExpedienteId, tramite.Etiqueta);
    }
}