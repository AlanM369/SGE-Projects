using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

public class BajaExpedienteUseCase(IExpedienteRepository expedienteRepositorio, ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion, IUnidadDeTrabajo uow)
{
    public BajaExpedienteResponse Ejecutar(BajaExpedienteRequest request)
    {   
        // 1. Verificamos permisos
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
            throw new AutorizacionException("El usuario no tiene permisos para eliminar expedientes.");

        // 2. Verificamos que el expediente exista 
        var expediente = expedienteRepositorio.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");

        // 3. Orquestación de la Baja en Cascada, primero los tramites asociados, luego el expediente
        // 1. Buscamos y eliminamos uno por uno todos los trámites asociados al expediente
        var tramitesAsociados = tramiteRepositorio.ObtenerPorExpedienteId(request.ExpedienteId);
        foreach (var tramite in tramitesAsociados)
        {
            tramiteRepositorio.Eliminar(tramite.Id);
        }

        // 4. Finalmente, borramos el expediente
        expedienteRepositorio.Eliminar(request.ExpedienteId);
        uow.Guardar();

        return new BajaExpedienteResponse();
    }
}

