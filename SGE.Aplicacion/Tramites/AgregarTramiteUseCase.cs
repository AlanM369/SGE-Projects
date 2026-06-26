using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Tramites;

public class AgregarTramiteUseCase (ITramiteRepository tramiteRepositorio, IExpedienteRepository expedienteRepositorio, IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado, IUnidadDeTrabajo uow)
{
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {   
        // 1. Verificamos la autorización del usuario
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.TramiteAlta))
            throw new AutorizacionException("El usuario no tiene permisos para crear trámites.");

        // 2. Validar que el expediente al que le queremos agregar el trámite exista
        var expediente = expedienteRepositorio.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"El expediente {request.ExpedienteId} no existe.");

        // 3. Dominio: Instanciamos el Value Object y luego la Entidad
        var contenidoVO = new ContenidoTramite(request.Contenido);
        var nuevoTramite = new Tramite(request.ExpedienteId, request.Etiqueta, contenidoVO, request.IdUsuario);

        // 4. Persistencia del Trámite
        tramiteRepositorio.Agregar(nuevoTramite);

        uow.Guardar();

        // 5.Le avisamos al actualizador que revise el expediente
        actualizadorEstado.Actualizar(request.ExpedienteId, request.IdUsuario);

         // El actualizador ya no llama a Modificar directamente sobre el repo,
         // solo marca el cambio en memoria. El Guardar() al final confirma todo.
        uow.Guardar();

        // 6. Retorno del DTO
        return new AgregarTramiteResponse(nuevoTramite.Id);
    }
}