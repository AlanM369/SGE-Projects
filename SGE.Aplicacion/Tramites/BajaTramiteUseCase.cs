using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;

namespace SGE.Aplicacion.Tramites;
public class BajaTramiteUseCase(ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado)
{
    public BajaTramiteResponse Ejecutar(BajaTramiteRequest request)
    {   
        // 1. Verificamos la autorización del usuario
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
            throw new AutorizacionException("El usuario no tiene permisos para eliminar trámites.");

         // 2. Validar que el tramite exista
        var tramite = tramiteRepositorio.ObtenerPorId(request.TramiteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el trámite con ID {request.TramiteId}");
        
        // Nos guardamos el ID antes de borrarlo
        // porque una vez eliminado, ya no podremos acceder a tramite.ExpedienteId
        Guid expedienteId = tramite.ExpedienteId;

        // 3. Persistencia: Eliminamos el tramite fisicamente
        tramiteRepositorio.Eliminar(request.TramiteId);

        // 4. Orquestación: Recalculamos el estado del expediente afectado
        // Si borramos el tramite de "Resolucion", el expediente podria volver a estado "ParaResolver"
        actualizadorEstado.Actualizar(expedienteId, request.IdUsuario);

        return new BajaTramiteResponse();
    }
}