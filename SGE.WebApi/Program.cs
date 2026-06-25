using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SGE.Infraestructura;
using SGE.WebApi;
using SGE.WebApi.Endpoints;
using SGE.Aplicacion;
using Microsoft.OpenApi;
using SGE.Infraestructura.Persistencia;

// -- FASE 1: CONFIGURACIÓN DEL BUILDER --
// Acá se registran todos los servicios en el contenedor de DI, antes de levantar la app

var builder = WebApplication.CreateBuilder(args);

// Capa de Infraestructura: EF Core + SgeContext, repositorios, JwtService, HashService, UoW
builder.Services.AddInfraestructura(builder.Configuration);

// Capa de Aplicación: los Casos de Uso (uno por cada acción del sistema)
builder.Services.AddAplicacion();

// Autenticación con JWT: se configura qué tiene que validar el middleware cuando llega un token
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var key = jwtSection["Key"]
    ?? throw new InvalidOperationException("La clave JWT no está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Se validan los 4 puntos clave
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSection["Issuer"],
            ValidAudience            = jwtSection["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Habilita el uso de [Authorize] / .RequireAuthorization() en los endpoints
builder.Services.AddAuthorization();

// Documentación OpenAPI + configuración del botón "Authorize" de Scalar, para poder pegar el token JWT y probar los endpoints protegidos desde ahí mismo.
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

// ProblemDetails: formato estándar de respuesta de error con el código HTTP que corresponde a cada una de las excepciones
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();

// -- FASE 2: CONSTRUCCIÓN DE LA APLICACIÓN --

var app = builder.Build();

//  Inicialización y Seed de la base de datos 
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
var hashService = scope.ServiceProvider.GetRequiredService<SGE.Aplicacion.Comun.IHashService>();
var uow = scope.ServiceProvider.GetRequiredService<SGE.Aplicacion.Comun.IUnidadDeTrabajo>();
SgeContextSeed.InicializarBaseDeDatos(context, hashService, uow);

// -- FASE 3: PIPELINE DE MIDDLEWARES --

//  El orden es CRÍTICO: Exception → Authentication → Authorization

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// -- FASE 4: MAPEO DE ENDPOINTS --

// Cada método de extensión registra las rutas de su propio archivo
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

// -- FASE 5: ARRANQUE DEL SERVIDOR KESTREL --

app.Run();