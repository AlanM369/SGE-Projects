using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class AgregarTramiteUseCase
{
    private readonly ITramiteRepository _tramiteRepositorio;
    private readonly IExpedienteRepository _expedienteRepositorio; // Lo necesitamos para verificar que el exp exista
    private readonly IAutorizacionService _autorizacion;
    private readonly ActualizacionEstadoExpedienteService _actualizadorEstado;

    public AgregarTramiteUseCase(ITramiteRepository tramiteRepositorio, IExpedienteRepository expedienteRepositorio,IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado)
    {
        _tramiteRepositorio = tramiteRepositorio;
        _expedienteRepositorio = expedienteRepositorio;
        _autorizacion = autorizacion;
        _actualizadorEstado = actualizadorEstado;
    }

    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {   
        // 1. Verificamos la autorización del usuario
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.TramiteAlta))
        {
            throw new AutorizacionException("El usuario no tiene permisos para crear trámites.");
        }

        // 2. Validar que el expediente al que le queremos agregar el trámite exista
        var expediente = _expedienteRepositorio.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"El expediente {request.ExpedienteId} no existe.");
        }

        // 3. Dominio: Instanciamos el Value Object y luego la Entidad
        var contenidoVO = new ContenidoTramite(request.Contenido);
        var nuevoTramite = new Tramite(request.ExpedienteId, request.Etiqueta, contenidoVO, request.IdUsuario);

        // 4. Persistencia del Trámite
        _tramiteRepositorio.Agregar(nuevoTramite);

        // 5.Le avisamos al actualizador que revise el expediente
        _actualizadorEstado.Actualizar(request.ExpedienteId, request.IdUsuario);

        // 6. Retorno del DTO
        return new AgregarTramiteResponse(nuevoTramite.Id);
    }
}
