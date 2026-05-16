using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepository;
    private readonly IAutorizacionService _autorizacionService;

    public CambiarEstadoExpedienteUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
    {
        _expedienteRepository = expedienteRepository;
        _autorizacionService = autorizacionService;
    }

    //Recibe el id del expediente, el nuevo estado y el id del usuario
    //Devuelve DTO con el resultado
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        //Chequea autorización
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para modificar el estado del expediente.");

        //Busca expediente
        //Si no encuenta devuelve null y lanza excepción
        var expediente = _expedienteRepository.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con Id {request.IdExpediente}.");

        //La entidad aplica el cambio
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        //Se reemplazan los datos viejos en el .txt
        _expedienteRepository.Modificar(expediente);

        //Devuelve el id y el nuevo estado
        return new CambiarEstadoExpedienteResponse(expediente.Id, expediente.Estado.ToString());
    }
}