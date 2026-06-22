using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion, IUnidadDeTrabajo uow)
{
    public void Ejecutar(CambiarEstadoRequest request)
    {
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("El usuario no tiene permisos para modificar el estado.");

        var expediente = repositorio.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}.");

        // Modificamos el estado a través de la entidad
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        repositorio.Modificar(expediente);
        uow.Guardar();
    }
}