using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGE.Infraestructura;
using SGE.WebApi;
using SGE.WebApi.Endpoints;

// ══════════════════════════════════════════════════════════════
//  FASE 1: CONFIGURACIÓN DEL BUILDER
// ══════════════════════════════════════════════════════════════

var builder = WebApplication.CreateBuilder(args);

// ─── Capa de Infraestructura (EF Core, repositorios, JWT, hash) ───
builder.Services.AddInfraestructura(builder.Configuration);

// ─── Capa de Aplicación (Casos de Uso) ───
builder.Services.AddAplicacion();

// ─── Autenticación con JWT Bearer ───
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var key = jwtSection["Key"]
    ?? throw new InvalidOperationException("La clave JWT no está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSection["Issuer"],
            ValidAudience            = jwtSection["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// ─── Autorización ───
builder.Services.AddAuthorization();

// ─── OpenAPI / Scalar ───
builder.Services.AddOpenApi();

// ══════════════════════════════════════════════════════════════
//  FASE 2: CONSTRUCCIÓN DE LA APLICACIÓN
// ══════════════════════════════════════════════════════════════

var app = builder.Build();

// ─── Inicialización y Seed de la base de datos ───
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
var hashService = scope.ServiceProvider.GetRequiredService<SGE.Aplicacion.Comun.IHashService>();
SgeContextSeed.InicializarBaseDeDatos(context, hashService);

// ══════════════════════════════════════════════════════════════
//  FASE 3: PIPELINE DE MIDDLEWARES
//  El orden es CRÍTICO: Exception → Authentication → Authorization
// ══════════════════════════════════════════════════════════════

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        if (exceptionHandlerFeature?.Error is null)
            return;

        var error = exceptionHandlerFeature.Error;

        var (statusCode, titulo) = error switch
        {
            SGE.Aplicacion.Comun.AutorizacionException        => (StatusCodes.Status403Forbidden,            "Acceso denegado"),
            SGE.Aplicacion.Comun.EntidadNoEncontradaException => (StatusCodes.Status404NotFound,             "Recurso no encontrado"),
            SGE.Aplicacion.Comun.EntidadDuplicadaException    => (StatusCodes.Status400BadRequest,           "Recurso duplicado"),
            SGE.Dominio.Comun.DominioException                => (StatusCodes.Status400BadRequest,           "Error de dominio"),
            _                                                 => (StatusCodes.Status500InternalServerError,  "Error interno del servidor")
        };

        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status   = statusCode,
            Title    = titulo,
            Detail   = error.Message,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseAuthentication();
app.UseAuthorization();

// ══════════════════════════════════════════════════════════════
//  FASE 4: MAPEO DE ENDPOINTS
// ══════════════════════════════════════════════════════════════

app.MapUsuariosEndpoints();
app.MapExpedientesEndpoints();
app.MapTramitesEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "SGE - Sistema de Gestión de Expedientes";
        options.AddHttpAuthentication("Bearer", auth =>
        {
            auth.Description = "Pegar el token JWT obtenido del endpoint /api/usuarios/login";
        });
    });
}

// ══════════════════════════════════════════════════════════════
//  FASE 5: ARRANQUE DEL SERVIDOR KESTREL
// ══════════════════════════════════════════════════════════════

app.Run();