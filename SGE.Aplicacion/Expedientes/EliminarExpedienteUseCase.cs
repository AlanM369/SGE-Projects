using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class EliminarExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepository;
    private readonly ITramiteRepository _tramiteRepository;
    private readonly IAutorizacionService _autorizacionService;

    public EliminarExpedienteUseCase(IExpedienteRepository expedienteRepository, ITramiteRepository tramiteRepository, IAutorizacionService autorizacionService)
    {
        _expedienteRepository = expedienteRepository;
        _tramiteRepository = tramiteRepository;
        _autorizacionService = autorizacionService;
    }

    //Devuelve void. Si no funciona lanza excepción
    public void Ejecutar(EliminarExpedienteRequest request)
    {
        // Verifica autorización
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
            throw new AutorizacionException("El usuario no tiene permiso para eliminar expedientes.");

        //Si el expediente es null lanza excepcion
        var expediente = _expedienteRepository.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con Id {request.IdExpediente}.");

        //Se obtienen y eliminan todos los trámites del expediente
        var tramites = _tramiteRepository.ObtenerPorExpedienteId(request.IdExpediente);
        foreach (var tramite in tramites)
            _tramiteRepository.Eliminar(tramite.Id);

        //Despues de eliminar los tramites, se elimina el expediente
        _expedienteRepository.Eliminar(request.IdExpediente);
    }
}