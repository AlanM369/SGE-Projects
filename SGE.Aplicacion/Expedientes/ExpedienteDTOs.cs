using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

// DTO para Listar: Contiene los datos "planos" listos para mostrar en pantalla
public record class ExpedienteDetalleDTOs(
    Guid Id, 
    string Caratula, 
    EstadoExpediente Estado, 
    DateTime FechaCreacion, 
    DateTime FechaUltimaModificacion
);

// DTO de entrada: Los datos exactos que la consola nos tiene que mandar.
// Pedimos el IdUsuario porque el TP exige validar permisos y registrar quién hace qué.
public record class AgregarExpedienteRequest(string Caratula, Guid IdUsuario);

// DTO de salida: Lo que le respondemos a la consola si todo sale bien.
// Devolver el ID generado es una buena práctica para que la consola sepa qué expediente se acaba de crear.
public record class AgregarExpedienteResponse(Guid IdExpediente);

// DTOs para Modificaciones
// DTO para modificar la carátula
public record class ModificarCaratulaRequest(Guid ExpedienteId, string NuevaCaratula, Guid IdUsuario);
// DTO para dar de baja
public record class BajaExpedienteRequest(Guid ExpedienteId, Guid IdUsuario);

public record class CambiarEstadoRequest(Guid ExpedienteId, EstadoExpediente NuevoEstado, Guid IdUsuario);

