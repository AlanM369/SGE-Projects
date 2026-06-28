using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Comun;

namespace SGE.WebApi;

// Traduce las excepciones de negocio en respuestas HTTP estandarizadas usando el formato ProblemDetails (RFC 7807).
// Retornar true al final le avisa a .NET que ya nos hicimos cargo del error y que no debe lanzar el 500 genérico.

public class ManejadorDeExcepcionesGlobales : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Evaluación del tipo de excepción capturada para determinar el código de estado HTTP y el título correspondientes
        var (statusCode, titulo) = exception switch
        {
            AutenticacionException       => (StatusCodes.Status401Unauthorized, "No autenticado"),
            AutorizacionException        => (StatusCodes.Status403Forbidden,          "Acceso denegado"),
            EntidadNoEncontradaException => (StatusCodes.Status404NotFound,           "Recurso no encontrado"),
            EntidadDuplicadaException    => (StatusCodes.Status400BadRequest,         "Recurso duplicado"),
            DominioException             => (StatusCodes.Status400BadRequest,         "Error de dominio"),
            _                            => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        // Construcción del objeto estructurado con los detalles técnicos del error
        var problemDetails = new ProblemDetails
        {
            Status   = statusCode,
            Title    = titulo,
            Detail   = exception.Message,
            Instance = httpContext.Request.Path
        };

        // Configuración de los metadatos de la respuesta HTTP e indicación del tipo de contenido específico
        httpContext.Response.StatusCode  = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        // Serialización y envío de los detalles del problema en formato JSON hacia el cliente de forma asíncrona
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
