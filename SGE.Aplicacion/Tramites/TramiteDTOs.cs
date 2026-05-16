using SGE.Dominio.Tramites; // Para ver el enum EtiquetaTramite
using SGE.Dominio.Expedientes; // Para ver el enum EstadoExpediente

namespace SGE.Aplicacion.Tramites;

// --- DTOs de Entrada (Requests) ---
// Para el alta, la consola nos tiene que decir a qué expediente va este trámite
public record class AgregarTramiteRequest(Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);

public record class ModificarTramiteRequest(Guid TramiteId, EtiquetaTramite NuevaEtiqueta, string NuevoContenido, Guid IdUsuario);

public record class BajaTramiteRequest(Guid TramiteId, Guid IdUsuario);

// --- DTOs de Salida (Responses y Detalles) ---
public record class AgregarTramiteResponse(Guid IdTramite);

public record class TramiteDetalleDTO(
    Guid Id, 
    Guid ExpedienteId, 
    EtiquetaTramite Etiqueta, 
    string Contenido, 
    DateTime FechaCreacion, 
    DateTime FechaUltimaModificacion
);



// DTOs para Consultas (Listados) para evitar que la entidad "escape" de la capa
//public record class TramiteDetalleDTO(Guid Id, Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, DateTime FechaCreacion, DateTime FechaUltimaModificacion);

//public record class ExpedienteDetalleDTO(Guid Id, string Caratula, EstadoExpediente Estado, DateTime FechaCreacion, DateTime FechaUltimaModificacion);