using System.Security.Claims;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.WebApi.Endpoints;

// Endpoints de Trámites, agrupados en una sola clase
public static class TramitesEndpoints
{
    public static void MapTramitesEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/tramites").WithTags("Tramites");

        //  ENDPOINTS DE LECTURA (solo token requerido)

        // GET /api/tramites/por-expediente/{expedienteId}
        // Lista todos los trámites de un expediente dado.
        grupo.MapGet("/por-expediente/{expedienteId:guid}", (
            Guid expedienteId,
            ListarTramitesPorExpedienteUseCase useCase) =>
        {
            var tramites = useCase.Ejecutar(new ListarTramitesPorExpedienteRequest(expedienteId));
            return Results.Ok(tramites);
        })
        .RequireAuthorization()
        .WithName("ListarTramitesPorExpediente")
        .WithSummary("Listar trámites de un expediente")
        .WithDescription("Devuelve todos los trámites asociados al expediente indicado. Solo lectura.");

        //  ENDPOINTS QUE MODIFICAN DATOS (token + permiso puntual de cada caso)

        // POST /api/tramites
        // Agrega un trámite a un expediente. Requiere permiso TramiteAlta.
        grupo.MapPost("/", (
            AgregarTramiteBodyRequest body,
            ClaimsPrincipal user,
            AgregarTramiteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new AgregarTramiteRequest(body.ExpedienteId, body.Etiqueta, body.Contenido, idUsuario);
            var respuesta = useCase.Ejecutar(request);
            return Results.Created($"/api/tramites/{respuesta.IdTramite}", respuesta);
        })
        .RequireAuthorization()
        .WithName("AgregarTramite")
        .WithSummary("Agregar un trámite a un expediente")
        .WithDescription("Requiere el permiso 'TramiteAlta'. Al agregar un trámite, el estado del expediente se recalcula automáticamente. Los valores de etiqueta son: Inicio = 0, PaseAEstudio = 1, PaseAResolucion = 2, Resolucion = 3.");

        // PUT /api/tramites/{id}
        // Modifica un trámite existente. Requiere permiso TramiteModificacion.
        grupo.MapPut("/{id:guid}", (
            Guid id,
            ModificarTramiteBodyRequest body,
            ClaimsPrincipal user,
            ModificarTramiteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new ModificarTramiteRequest(id, body.NuevaEtiqueta, body.NuevoContenido, idUsuario);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("ModificarTramite")
        .WithSummary("Modificar un trámite")
        .WithDescription("Requiere el permiso 'TramiteModificacion'. Actualizar la etiqueta puede cambiar el estado del expediente padre automáticamente.");

        // DELETE /api/tramites/{id}
        // Da de baja un trámite. Requiere permiso TramiteBaja.
        // Nota: tener ExpedienteBaja implica implícitamente TramiteBaja.
        grupo.MapDelete("/{id:guid}", (
            Guid id,
            ClaimsPrincipal user,
            BajaTramiteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new BajaTramiteRequest(id, idUsuario);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("BajaTramite")
        .WithSummary("Eliminar un trámite")
        .WithDescription("Requiere el permiso 'TramiteBaja'. Recuerda: tener 'ExpedienteBaja' implica automáticamente 'TramiteBaja'.");
    }

    // Mismo helper que en ExpedientesEndpoints
    private static Guid ObtenerUserIdDelToken(ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido: no contiene el identificador de usuario.");

        return Guid.Parse(valor);
    }
}

// Bodies que recibe cada endpoint mutativo.
public record AgregarTramiteBodyRequest(Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido);
public record ModificarTramiteBodyRequest(EtiquetaTramite NuevaEtiqueta, string NuevoContenido);
