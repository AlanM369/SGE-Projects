using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class ModificarTramiteUseCase(ITramiteRepository tramiteRepositorio, IAutorizacionService autorizacion, ActualizacionEstadoExpedienteService actualizadorEstado)
{
    public void Ejecutar(ModificarTramiteRequest request)
    {
         // 1. Verificamos la autorización del usuario
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.TramiteModificacion))
            throw new AutorizacionException("El usuario no tiene permisos para modificar trámites.");

        // 2. Validar que el tramite exista
        var tramite = tramiteRepositorio.ObtenerPorId(request.TramiteId)
            ?? throw new EntidadNoEncontradaException($"No se encontró el trámite con ID {request.TramiteId}");

        // 3. Dominio: Instanciamos el nuevo Value Object de contenido y modificamos
        var nuevoContenidoVO = new ContenidoTramite(request.NuevoContenido);
        
        // La entidad Tramite es la responsable de actualizar sus propios datos y su fecha de modificación
        tramite.ModificarTramite(request.NuevaEtiqueta, nuevoContenidoVO, request.IdUsuario);
        
        // 4. Persistencia
        tramiteRepositorio.Modificar(tramite);
 
        // 5. Orquestación: Forzamos la revisión del estado del expediente 
        // (por si el usuario cambió una etiqueta "PaseAEstudio" por una de "Resolucion")
        actualizadorEstado.Actualizar(tramite.ExpedienteId, request.IdUsuario);
    }
}