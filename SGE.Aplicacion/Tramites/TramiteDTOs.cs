using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

// Definimos los DTOs que se usarán para transferir datos entre capas

// DTO para agregar un trámite.
public record AgregarTramiteRequest(Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);
public record AgregarTramiteResponse(Guid Id, Guid ExpedienteId, EtiquetaTramite Etiqueta);

// DTO para eliminar un trámite.
public record EliminarTramiteRequest(Guid IdTramite, Guid IdUsuario);

// DTO para modificar un trámite.
public record ModificarTramiteRequest(Guid IdTramite, EtiquetaTramite NuevaEtiqueta, string NuevoContenido, Guid IdUsuario);
public record ModificarTramiteResponse(Guid Id, EtiquetaTramite Etiqueta, string Contenido);

// DTO para obtener un trámite.
public record TramiteResponse(Guid Id, Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, DateTime FechaCreacion);