using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGE.WebApi;
using SGE.WebApi.Endpoints;

// ══════════════════════════════════════════════════════════════
//  FASE 1: CONFIGURACIÓN DEL BUILDER
//  Aquí registramos todos los servicios en el contenedor de DI
//  antes de construir la aplicación.
// ══════════════════════════════════════════════════════════════

var builder = WebApplication.CreateBuilder(args);

// ─── Capa de Infraestructura (EF Core, repositorios, JWT, hash) ───
builder.Services.AddInfraestructura(builder.Configuration);

// ─── Capa de Aplicación (Casos de Uso) ───
builder.Services.AddAplicacion();

// ─── Autenticación con JWT Bearer ───
// Configuramos el middleware para que valide los tokens de forma estricta.
// El token lleva el UserId dentro del claim ClaimTypes.NameIdentifier.
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
// AddOpenApi registra los metadatos de los endpoints para que
// Scalar pueda generar la interfaz de documentación interactiva.
builder.Services.AddOpenApi();

// ─── Manejador de Excepciones Global ───
// Registramos la clase que traduce excepciones a ProblemDetails.
// AddProblemDetails agrega soporte para el formato estándar RFC 7807.
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();
builder.Services.AddProblemDetails();

// ══════════════════════════════════════════════════════════════
//  FASE 2: CONSTRUCCIÓN DE LA APLICACIÓN
//  A partir de aquí no se pueden agregar más servicios.
// ══════════════════════════════════════════════════════════════

var app = builder.Build();

// ─── Inicialización y Seed de la base de datos ───
// Si la BD no existe, se crea con EnsureCreated() y se cargan
// el administrador semilla y los usuarios de prueba.
app.InicializarBaseDeDatos();

// ══════════════════════════════════════════════════════════════
//  FASE 3: PIPELINE DE MIDDLEWARES
//  El orden es CRÍTICO: Exception → Authentication → Authorization
// ══════════════════════════════════════════════════════════════

// 1. Manejador Global de Excepciones
// ManejadorDeExcepcionesGlobales intercepta DominioException,
// AutorizacionException y EntidadNoEncontradaException y retorna
// ProblemDetails con el código HTTP correspondiente (400, 403, 404).
app.UseExceptionHandler();

// 2. Middleware de Autenticación (verifica el token JWT)
app.UseAuthentication();

// 3. Middleware de Autorización (verifica que el endpoint requiera auth y la aplica)
app.UseAuthorization();

// ══════════════════════════════════════════════════════════════
//  FASE 4: MAPEO DE ENDPOINTS
//  Organizados en clases estáticas por contexto de negocio,
//  usando IEndpointRouteBuilder como indica el TP2.
// ══════════════════════════════════════════════════════════════

app.MapUsuariosEndpoints();
app.MapExpedientesEndpoints();
app.MapTramitesEndpoints();

// ─── Scalar: UI de documentación interactiva ───
// Solo disponible en Development para no exponer la documentación en producción.
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