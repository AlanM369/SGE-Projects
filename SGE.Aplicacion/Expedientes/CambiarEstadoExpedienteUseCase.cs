using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

// Caso de Uso para la modificación manual del estado de un expediente
public class CambiarEstadoExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion, IUnidadDeTrabajo uow)
{
    public CambiarEstadoResponse Ejecutar(CambiarEstadoRequest request)
    {
        // 1. Validamos los permisos del usuario antes de operar
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
            throw new AutorizacionException("El usuario no tiene permisos para modificar el estado.");

        // 2. Buscamos la entidad cruda en la infraestructura
        var expediente = repositorio.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}.");

        // 3. Modificamos el estado a través de la entidad de dominio
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        // 4. Marcamos los cambios y los confirmamos de forma atómica
        repositorio.Modificar(expediente);
        uow.Guardar();

        // 5. Devolvemos la respuesta vacía
        return new CambiarEstadoResponse();
    }
}