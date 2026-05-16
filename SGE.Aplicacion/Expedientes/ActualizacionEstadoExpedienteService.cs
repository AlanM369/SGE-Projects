using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Expedientes;

// Esta clase centraliza la lógica de orquestación para actualizar el estado
public class ActualizacionEstadoExpedienteService(IExpedienteRepository expedienteRepositorio, ITramiteRepository tramiteRepositorio)
{
    public void Actualizar(Guid expedienteId, Guid idUsuario)
    {
        // Buscamos el expediente
        var expediente = expedienteRepositorio.ObtenerPorId(expedienteId)
            ?? throw new EntidadNoEncontradaException("Expediente no encontrado");

        // Buscamos todos los trámites de este expediente
        var tramites = tramiteRepositorio.ObtenerPorExpedienteId(expedienteId);
        
        // Buscamos el último trámite ingresado ordenando por fecha
        // (FirstOrDefault devuelve el primero de la lista, o null si la lista está vacía)
        var ultimoTramite = tramites.OrderByDescending(t => t.FechaCreacion).FirstOrDefault();
        
        // Extraemos la etiqueta del ultimo tramite(si no hay trámites, enviamos null)
        EtiquetaTramite? ultimaEtiqueta = ultimoTramite?.Etiqueta;

        // Le pedimos a la entidad de Dominio que aplique sus reglas y cambie su estado si corresponde
        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, idUsuario);

        // Solo vamos al repositorio a guardar si el dominio nos confirmó que hubo un cambio real
        if (cambio)
            expedienteRepositorio.Modificar(expediente);
    }
}