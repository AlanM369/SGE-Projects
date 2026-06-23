using System.Security.Claims;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;

namespace SGE.WebApi.Endpoints;

/// <summary>
/// Endpoints para la gestión de Expedientes.
/// Agrupados con .WithTags("Expedientes") para organizar la interfaz de Scalar.
/// </summary>
public static class ExpedientesEndpoints
{
    public static void MapExpedientesEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/expedientes").WithTags("Expedientes");

        // ─────────────────────────────────────────
        //  ENDPOINTS DE LECTURA (token requerido)
        // ─────────────────────────────────────────

        // GET /api/expedientes
        // Lista todos los expedientes. Cualquier usuario autenticado puede consultar.
        grupo.MapGet("/", (ListarExpedientesUseCase useCase) =>
        {
            var expedientes = useCase.Ejecutar();
            return Results.Ok(expedientes);
        })
        .RequireAuthorization()
        .WithName("ListarExpedientes")
        .WithSummary("Listar todos los expedientes")
        .WithDescription("Devuelve la colección completa de expedientes (sin trámites). Solo lectura, cualquier usuario autenticado puede acceder.");

        // GET /api/expedientes/{id}
        // Obtiene un expediente con todos sus trámites asociados.
        grupo.MapGet("/{id:guid}", (
            Guid id,
            ObtenerExpedienteConTramitesUseCase useCase) =>
        {
            var resultado = useCase.Ejecutar(id);
            return Results.Ok(resultado);
        })
        .RequireAuthorization()
        .WithName("ObtenerExpedienteConTramites")
        .WithSummary("Obtener expediente con sus trámites")
        .WithDescription("Devuelve el detalle completo de un expediente junto con la colección de trámites que posee.");

        // ─────────────────────────────────────────
        //  ENDPOINTS MUTANTES (token + permisos)
        // ─────────────────────────────────────────

        // POST /api/expedientes
        // Crea un nuevo expediente. Requiere permiso ExpedienteAlta.
        grupo.MapPost("/", (
            AgregarExpedienteBodyRequest body,
            ClaimsPrincipal user,
            AgregarExpedienteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new AgregarExpedienteRequest(body.Caratula, idUsuario);
            var respuesta = useCase.Ejecutar(request);
            return Results.Created($"/api/expedientes/{respuesta.IdExpediente}", respuesta);
        })
        .RequireAuthorization()
        .WithName("AgregarExpediente")
        .WithSummary("Crear un nuevo expediente")
        .WithDescription("Requiere el permiso 'ExpedienteAlta'. El ID del usuario se obtiene automáticamente del token JWT.");

        // PUT /api/expedientes/{id}/caratula
        // Modifica la carátula de un expediente. Requiere permiso ExpedienteModificacion.
        grupo.MapPut("/{id:guid}/caratula", (
            Guid id,
            ModificarCaratulaBodyRequest body,
            ClaimsPrincipal user,
            ModificarCaratulaExpedienteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new ModificarCaratulaRequest(id, body.NuevaCaratula, idUsuario);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("ModificarCaratulaExpediente")
        .WithSummary("Modificar la carátula de un expediente")
        .WithDescription("Requiere el permiso 'ExpedienteModificacion'.");

        // PATCH /api/expedientes/{id}/estado
        // Cambia el estado manualmente. Requiere permiso ExpedienteModificacion.
        grupo.MapPatch("/{id:guid}/estado", (
            Guid id,
            CambiarEstadoBodyRequest body,
            ClaimsPrincipal user,
            CambiarEstadoExpedienteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new CambiarEstadoRequest(id, body.NuevoEstado, idUsuario);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("CambiarEstadoExpediente")
        .WithSummary("Cambiar el estado de un expediente manualmente")
        .WithDescription("Requiere el permiso 'ExpedienteModificacion'. Los valores válidos de estado son: Creado = 0, ParaResolver = 1, EnProceso = 2, Resuelto = 3.");

        // DELETE /api/expedientes/{id}
        // Da de baja en cascada (expediente + sus trámites). Requiere permiso ExpedienteBaja.
        grupo.MapDelete("/{id:guid}", (
            Guid id,
            ClaimsPrincipal user,
            BajaExpedienteUseCase useCase) =>
        {
            var idUsuario = ObtenerUserIdDelToken(user);
            var request = new BajaExpedienteRequest(id, idUsuario);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("BajaExpediente")
        .WithSummary("Dar de baja un expediente (con cascada)")
        .WithDescription("Elimina el expediente y todos sus trámites asociados. Requiere el permiso 'ExpedienteBaja'.");
    }

    private static Guid ObtenerUserIdDelToken(ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido: no contiene el identificador de usuario.");

        return Guid.Parse(valor);
    }
}

// ─── Request bodies auxiliares ───

public record AgregarExpedienteBodyRequest(string Caratula);
public record ModificarCaratulaBodyRequest(string NuevaCaratula);
public record CambiarEstadoBodyRequest(EstadoExpediente NuevoEstado);
