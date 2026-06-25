using System.Security.Claims;
using SGE.Aplicacion.Usuarios;

namespace SGE.WebApi.Endpoints;

// Endpoints para autenticación y gestión de usuarios.
public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/usuarios").WithTags("Usuarios");

        //  ENDPOINTS PÚBLICOS (sin token requerido)

        // POST /api/usuarios/registrar
        // Registro abierto para cualquier persona.
        grupo.MapPost("/registrar", (
            RegistrarUsuarioRequest request,
            RegistrarUsuarioUseCase useCase) =>
        {
            var respuesta = useCase.Ejecutar(request);
            return Results.Created($"/api/usuarios/{respuesta.Id}", respuesta);
        })
        .WithName("RegistrarUsuario")
        .WithSummary("Registrar un nuevo usuario")
        .WithDescription("Registro abierto. Por defecto el usuario no tiene permisos de mutación.");

        // POST /api/usuarios/login
        // Valida credenciales y devuelve un token JWT.
        grupo.MapPost("/login", (
            LoginRequest request,
            LoginUseCase useCase) =>
        {
            var respuesta = useCase.Ejecutar(request);
            return Results.Ok(respuesta);
        })
        .WithName("Login")
        .WithSummary("Iniciar sesión")
        .WithDescription("Devuelve un token JWT. Usar ese token en el botón 'Authorize' de Scalar para acceder a los endpoints protegidos.");

        //  ENDPOINTS PROTEGIDOS (token requerido)

        // PUT /api/usuarios/mis-datos
        // Cualquier usuario autenticado puede actualizar sus propios datos.
        // El IdUsuario se extrae del token JWT (ClaimTypes.NameIdentifier).
        grupo.MapPut("/mis-datos", (
            ModificarMisDatosBodyRequest body,
            ClaimsPrincipal user,
            ModificarMisDatosUseCase useCase) =>
        {
            // Extraemos el UserId del token; nunca del body (seguridad)
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Token inválido: no contiene el identificador de usuario.");

            var idUsuario = Guid.Parse(userIdString);

            var request = new ModificarMisDatosRequest(
                idUsuario,
                body.NuevoNombre,
                body.NuevoCorreo,
                body.NuevaContrasena
            );

            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("ModificarMisDatos")
        .WithSummary("Modificar mis datos personales")
        .WithDescription("Permite al usuario autenticado actualizar su nombre, correo y/o contraseña. El ID se obtiene del token JWT.");

        //  ENDPOINTS EXCLUSIVOS DEL ADMINISTRADOR

        // GET /api/usuarios
        // Lista todos los usuarios del sistema.
        grupo.MapGet("/", (
            ClaimsPrincipal user,
            ListarUsuariosUseCase useCase) =>
        {
            var idAdmin = ObtenerUserIdDelToken(user);
            var usuarios = useCase.Ejecutar(idAdmin);
            return Results.Ok(usuarios);
        })
        .RequireAuthorization()
        .WithName("ListarUsuarios")
        .WithSummary("[Admin] Listar todos los usuarios")
        .WithDescription("Devuelve la lista completa de usuarios. Solo accesible para administradores.");

        // DELETE /api/usuarios/{id}
        // Da de baja a un usuario.
        grupo.MapDelete("/{id:guid}", (
            Guid id,
            ClaimsPrincipal user,
            EliminarUsuarioUseCase useCase) =>
        {
            var idAdmin = ObtenerUserIdDelToken(user);
            var request = new EliminarUsuarioRequest(idAdmin, id);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("EliminarUsuario")
        .WithSummary("[Admin] Eliminar un usuario")
        .WithDescription("Da de baja a un usuario del sistema. Solo accesible para administradores.");

        // PUT /api/usuarios/{id}/permisos
        // Modifica los permisos de un usuario.
        grupo.MapPut("/{id:guid}/permisos", (
            Guid id,
            ModificarPermisosBodyRequest body,
            ClaimsPrincipal user,
            ModificarPermisosUsuarioUseCase useCase) =>
        {
            var idAdmin = ObtenerUserIdDelToken(user);
            var request = new ModificarPermisosRequest(idAdmin, id, body.NuevosPermisos);
            useCase.Ejecutar(request);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("ModificarPermisosUsuario")
        .WithSummary("[Admin] Modificar permisos de un usuario")
        .WithDescription("Reemplaza completamente los permisos del usuario indicado. Solo accesible para administradores.");
    }

    // Helper para no repetir esta misma extracción en cada endpoint
    private static Guid ObtenerUserIdDelToken(ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido: no contiene el identificador de usuario.");

        return Guid.Parse(valor);
    }
}

// Separamos el body del endpoint para no incluir el IdUsuario, (ese siempre viene del token, nunca del cliente).
public record ModificarMisDatosBodyRequest(string NuevoNombre, string NuevoCorreo, string? NuevaContrasena);
public record ModificarPermisosBodyRequest(List<SGE.Dominio.Autorizacion.Permiso> NuevosPermisos);
