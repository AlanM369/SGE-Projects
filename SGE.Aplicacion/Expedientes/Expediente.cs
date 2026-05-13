using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

// DTOs para Alta
public record class AgregarExpedienteRequest(string Caratula, Guid IdUsuario);
public record class AgregarExpedienteResponse(Guid IdExpediente);

// DTOs para Modificaciones
public record class ModificarCaratulaRequest(Guid ExpedienteId, string NuevaCaratula, Guid IdUsuario);
public record class CambiarEstadoRequest(Guid ExpedienteId, EstadoExpediente NuevoEstado, Guid IdUsuario);

// DTOs para Baja
public record class BajaExpedienteRequest(Guid ExpedienteId, Guid IdUsuario);