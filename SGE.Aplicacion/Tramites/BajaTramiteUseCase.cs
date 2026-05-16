using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Aplicacion.Expedientes;

namespace SGE.Aplicacion.Tramites;

public class BajaTramiteUseCase
{
    private readonly ITramiteRepository _tramiteRepositorio;
    private readonly IAutorizacionService _autorizacion;
    private readonly ActualizacionEstadoExpedienteService _actualizadorEstado;

    public BajaTramiteUseCase(ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado)
    {
        _tramiteRepositorio = tramiteRepositorio;
        _autorizacion = autorizacion;
        _actualizadorEstado = actualizadorEstado;
    }

    public void Ejecutar(BajaTramiteRequest request)
    {   
        // 1. Verificamos la autorización del usuario
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
        {
            throw new AutorizacionException("El usuario no tiene permisos para eliminar trámites.");
        }

         // 2. Validar que el tramite exista
        var tramite = _tramiteRepositorio.ObtenerPorId(request.TramiteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el trámite con ID {request.TramiteId}");
        
        // Nos guardamos el ID antes de borrarlo
        // porque una vez eliminado, ya no podremos acceder a tramite.ExpedienteId
        Guid expedienteId = tramite.ExpedienteId;

        // 3. Persistencia: Eliminamos el trámite físicamente
        _tramiteRepositorio.Eliminar(request.TramiteId);

        // 4. Orquestación: Recalculamos el estado del expediente afectado
        // Si borramos el trámite de "Resolución", el expediente podría volver a estado "ParaResolver"
        _actualizadorEstado.Actualizar(expedienteId, request.IdUsuario);
    }
}