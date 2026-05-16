using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Aplicacion.Expedientes;

namespace SGE.Aplicacion.Tramites;
public class BajaTramiteUseCase(ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado)
{
    public void Ejecutar(BajaTramiteRequest request)
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

        // 3. Persistencia: Eliminamos el trámite físicamente
        tramiteRepositorio.Eliminar(request.TramiteId);

        // 4. Orquestación: Recalculamos el estado del expediente afectado
        // Si borramos el trámite de "Resolución", el expediente podría volver a estado "ParaResolver"
        actualizadorEstado.Actualizar(expedienteId, request.IdUsuario);
    }
}