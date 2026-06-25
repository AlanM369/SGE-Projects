using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGE.Infraestructura;
using SGE.WebApi;
using SGE.WebApi.Endpoints;
using SGE.Aplicacion;
using Microsoft.OpenApi;

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
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Pegar el token JWT obtenido del endpoint /api/usuarios/login"
            }
        };
        return Task.CompletedTask;
    });
});

// ─── ProblemDetails + Manejador global de excepciones ───
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();

// ══════════════════════════════════════════════════════════════
//  FASE 2: CONSTRUCCIÓN DE LA APLICACIÓN
// ══════════════════════════════════════════════════════════════

var app = builder.Build();

// ─── Inicialización y Seed de la base de datos ───
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
var hashService = scope.ServiceProvider.GetRequiredService<SGE.Aplicacion.Comun.IHashService>();
var uow = scope.ServiceProvider.GetRequiredService<SGE.Aplicacion.Comun.IUnidadDeTrabajo>();
SgeContextSeed.InicializarBaseDeDatos(context, hashService, uow);
// ══════════════════════════════════════════════════════════════
//  FASE 3: PIPELINE DE MIDDLEWARES
//  El orden es CRÍTICO: Exception → Authentication → Authorization
// ══════════════════════════════════════════════════════════════

app.UseExceptionHandler();
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
        options.AddPreferredSecuritySchemes("Bearer");
        options.AddHttpAuthentication("Bearer", auth =>
        {
            auth.Description = "Pegar el token JWT obtenido del endpoint /api/usuarios/login";
        });
        options.WithPersistentAuthentication();
    });
}

// ══════════════════════════════════════════════════════════════
//  FASE 5: ARRANQUE DEL SERVIDOR KESTREL
// ══════════════════════════════════════════════════════════════

app.Run();