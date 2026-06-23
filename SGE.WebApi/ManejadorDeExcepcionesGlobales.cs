using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Comun;

namespace SGE.WebApi;

/// <summary>
/// Traduce las excepciones de negocio en respuestas HTTP estandarizadas
/// usando el formato ProblemDetails (RFC 7807).
/// Retornar true al final le avisa a .NET que ya nos hicimos cargo del error
/// y que no debe lanzar el 500 genérico.
/// </summary>
public class ManejadorDeExcepcionesGlobales : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, titulo) = exception switch
        {
            AutorizacionException        => (StatusCodes.Status403Forbidden,          "Acceso denegado"),
            EntidadNoEncontradaException => (StatusCodes.Status404NotFound,           "Recurso no encontrado"),
            EntidadDuplicadaException    => (StatusCodes.Status400BadRequest,         "Recurso duplicado"),
            DominioException             => (StatusCodes.Status400BadRequest,         "Error de dominio"),
            _                            => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var problemDetails = new ProblemDetails
        {
            Status   = statusCode,
            Title    = titulo,
            Detail   = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode  = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
