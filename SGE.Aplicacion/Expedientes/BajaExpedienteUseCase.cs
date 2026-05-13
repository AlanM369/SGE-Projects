using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;

namespace SGE.Aplicacion.Expedientes;

public class BajaExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepo;
    private readonly ITramiteRepository _tramiteRepo;
    private readonly IAutorizacionService _auth;

    // Inyectamos ambos repositorios
    public BajaExpedienteUseCase(IExpedienteRepository expedienteRepo, ITramiteRepository tramiteRepo, IAutorizacionService auth)
    {
        _expedienteRepo = expedienteRepo;
        _tramiteRepo = tramiteRepo;
        _auth = auth;
    }

    public void Ejecutar(BajaExpedienteRequest request)
    {
        if (!_auth.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
        {
            throw new AutorizacionException("El usuario no tiene permisos para eliminar expedientes.");
        }

        var expediente = _expedienteRepo.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // 1. Buscamos y eliminamos primero todos los trámites asociados
        var tramitesAsociados = _tramiteRepo.ObtenerPorExpedienteId(request.ExpedienteId);
        foreach (var tramite in tramitesAsociados)
        {
            _tramiteRepo.Eliminar(tramite.Id);
        }

        // 2. Finalmente, eliminamos el expediente
        _expedienteRepo.Eliminar(request.ExpedienteId);
    }
}