using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepository;
    private readonly IAutorizacionService _autorizacionService;

    public ModificarCaratulaExpedienteUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
    {
        _expedienteRepository = expedienteRepository;
        _autorizacionService = autorizacionService;
    }

    //Recibe el id del expediente, la nueva carátula y el id del usuario
    //Devuelve DTO con resultado
    public ModificarCaratulaExpedienteResponse Ejecutar(ModificarCaratulaExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para modificar expedientes.");

        //Busca el expediente
        //Si devuelve null, el id no existe y lanza excepcion
        var expediente = _expedienteRepository.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con Id {request.IdExpediente}.");

        //Se crea nuevo ValueObject y se reemplazan los datos viejos
        var nuevaCaratula = new Caratula(request.NuevaCaratula);
        expediente.ModificarCaratula(nuevaCaratula, request.IdUsuario);

        _expedienteRepository.Modificar(expediente);

        return new ModificarCaratulaExpedienteResponse(expediente.Id, expediente.Caratula!.Texto);
    }
}