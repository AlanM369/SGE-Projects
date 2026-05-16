using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

//record -> solo transporta datos

//ALTA
public record AgregarExpedienteRequest(string Caratula, Guid IdUsuario);
public record AgregarExpedienteResponse(Guid Id, string Caratula, string Estado, DateTime FechaCreacion);

//BAJA
public record EliminarExpedienteRequest(Guid IdExpediente, Guid IdUsuario);

//MODIFICAR, entrada/salida para corregir caratula
public record ModificarCaratulaExpedienteRequest(Guid IdExpediente, string NuevaCaratula, Guid IdUsuario);
public record ModificarCaratulaExpedienteResponse(Guid Id, string NuevaCaratula);

//CAMBIAR
public record CambiarEstadoExpedienteRequest(Guid IdExpediente, EstadoExpediente NuevoEstado, Guid IdUsuario);
public record CambiarEstadoExpedienteResponse(Guid Id, string NuevoEstado);

//LISTAR, solo de salida
public record ListarExpedientesResponse(Guid Id, string Caratula, string Estado, DateTime FechaCreacion, DateTime FechaUltimaModificacion);