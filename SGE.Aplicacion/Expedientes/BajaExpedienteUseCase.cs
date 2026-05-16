using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;

namespace SGE.Aplicacion.Expedientes;

public class BajaExpedienteUseCase
{
    private readonly IExpedienteRepository _expedienteRepositorio;
    private readonly ITramiteRepository _tramiteRepositorio; // Necesitamos este para borrar los trámites
    private readonly IAutorizacionService _autorizacion;

    // Inyectamos ambos repositorios
    public BajaExpedienteUseCase(IExpedienteRepository expedienteRepositorio, ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion)
    {
        _expedienteRepositorio = expedienteRepositorio;
        _tramiteRepositorio = tramiteRepositorio;
        _autorizacion = autorizacion;
    }

    public void Ejecutar(BajaExpedienteRequest request)
    {   
        // 1. Verificamos permisos
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
        {
            throw new AutorizacionException("El usuario no tiene permisos para eliminar expedientes.");
        }
        // 2. Verificamos que el expediente exista 
        var expediente = _expedienteRepositorio.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // 3. Orquestación de la Baja en Cascada
        // 1. Buscamos y eliminamos uno por uno todos los trámites asociados al expediente
        var tramitesAsociados = _tramiteRepositorio.ObtenerPorExpedienteId(request.ExpedienteId);
        foreach (var tramite in tramitesAsociados)
        {
            _tramiteRepositorio.Eliminar(tramite.Id);
        }

        // 4. Finalmente, borramos el expediente
        _expedienteRepositorio.Eliminar(request.ExpedienteId);
    }
}