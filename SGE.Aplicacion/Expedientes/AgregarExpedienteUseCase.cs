using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepository;
    private readonly IAutorizacionService _autorizacionService;

    public AgregarExpedienteUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
    {
        _expedienteRepository = expedienteRepository;
        _autorizacionService = autorizacionService;
    }

    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
            throw new AutorizacionException("El usuario no tiene permiso para crear expedientes.");

        // El Value Object Caratula valida internamente. Si falla, lanza DominioException.
        var caratula = new Caratula(request.Caratula);
        var expediente = new Expediente(caratula, request.IdUsuario);

        _expedienteRepository.Agregar(expediente);

        return new AgregarExpedienteResponse(
            expediente.Id,
            expediente.Caratula!.Texto,
            expediente.Estado.ToString(),
            expediente.FechaCreacion);
    }
}