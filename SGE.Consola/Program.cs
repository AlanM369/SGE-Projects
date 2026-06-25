using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Infraestructura;
using SGE.Infraestructura.Repositorios;
using SGE.Infraestructura.Persistencia;
using SGE.Infraestructura.Seguridad;


// ─── COMPOSITION ROOT ────────────────────────────────────────────────────────
// Único lugar en todo el sistema donde se crean instancias concretas.
// Los casos de uso solo conocen interfaces, nunca estas clases concretas.

// ─── Autorizacion y pruebas ──────────────────────────────────────────────────────────────── 

// 1. Instanciamos el contexto de la base de datos SQLite
var contexto = new SgeContext();

// Opcional: Nos aseguramos de que la base de datos exista (esto te sirve para el seeding luego)
contexto.Database.EnsureCreated();

// 2. Instanciamos el repositorio de usuarios pasándole el contexto
var usuarioRepository = new UsuarioRepository(contexto);
var autorizacionService = new AutorizacionService(usuarioRepository);

var uow = new UnidadDeTrabajo(contexto);
//Fin

// Infraestructura
var expedienteRepository    = new ExpedienteRepository(contexto);
var tramiteRepository       = new TramiteRepository(contexto);
var actualizarEstado = new ActualizacionEstadoExpedienteService(expedienteRepository, tramiteRepository);

// Casos de uso de Expedientes
var agregarExpediente       = new AgregarExpedienteUseCase(expedienteRepository, autorizacionService, uow);
var eliminarExpediente      = new BajaExpedienteUseCase(expedienteRepository, tramiteRepository, autorizacionService, uow);
var modificarCaratula       = new ModificarCaratulaExpedienteUseCase(expedienteRepository, autorizacionService, uow);
var cambiarEstado           = new CambiarEstadoExpedienteUseCase(expedienteRepository, autorizacionService, uow);
var listarExpedientes       = new ListarExpedientesUseCase(expedienteRepository);

// Casos de uso de Trámites (completar con los de tu compañero)
var agregarTramite          = new AgregarTramiteUseCase(tramiteRepository, expedienteRepository, autorizacionService, actualizarEstado, uow);
var eliminarTramite         = new BajaTramiteUseCase(tramiteRepository,autorizacionService, actualizarEstado, uow);
var modificarTramite        = new ModificarTramiteUseCase(tramiteRepository, autorizacionService, actualizarEstado, uow);
var listarTramites          = new ListarTramitesPorExpedienteUseCase(tramiteRepository);

var idUsuario = Guid.NewGuid(); // Simulamos un usuario logueado




// ─── CAMINO FELIZ ─────────────────────────────────────────────────────────────

Console.WriteLine("=== 1. Agregar un expediente válido ===");
try
{
    var request = new AgregarExpedienteRequest("Expediente de prueba", idUsuario);
    var response = agregarExpediente.Ejecutar(request);
    Console.WriteLine($"[Éxito] Expediente creado. Id: {response.IdExpediente}\n");
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 2. Listar expedientes ===");
try
{
    var expedientes = listarExpedientes.Ejecutar(new ListarExpedientesRequest());
    foreach (var e in expedientes)
        Console.WriteLine($"> Id: {e.Id} | Carátula: {e.Caratula} | Estado: {e.Estado}");
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 3. Agregar un trámite y verificar cambio de estado automático ===");
try
{
    // Primero obtenemos el id del expediente que creamos antes
    var expedientes = listarExpedientes.Ejecutar(new ListarExpedientesRequest()).ToList();
    var idExpediente = expedientes.First().Id;

    // Agregamos un trámite con etiqueta PaseAEstudio → debe cambiar el estado a ParaResolver
    var request = new AgregarTramiteRequest(idExpediente, EtiquetaTramite.PaseAEstudio, "Pase a estudio del expediente", idUsuario);
    var response = agregarTramite.Ejecutar(request);
    Console.WriteLine($"[Éxito] Trámite agregado. Id: {response.IdTramite} | Etiqueta: {request.Etiqueta}");

    // Verificamos que el estado del expediente cambió automáticamente
    var expedienteActualizado = listarExpedientes.Ejecutar(new ListarExpedientesRequest()).First(e => e.Id == idExpediente);
    Console.WriteLine($"[Verificación] Estado del expediente: {expedienteActualizado.Estado} (esperado: ParaResolver)\n");
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 4. Cambiar estado manualmente ===");
try
{
    var idExpediente = listarExpedientes.Ejecutar(new ListarExpedientesRequest()).First().Id;
    var request = new CambiarEstadoRequest(idExpediente, EstadoExpediente.EnNotificacion, idUsuario);
    cambiarEstado.Ejecutar(request);
    Console.WriteLine($"[Éxito] Estado cambiado a: {request.NuevoEstado}\n");
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 5. Modificar carátula ===");
try
{
    var idExpediente = listarExpedientes.Ejecutar(new ListarExpedientesRequest()).First().Id;
    var request = new ModificarCaratulaRequest(idExpediente, "Carátula corregida", idUsuario);
    modificarCaratula.Ejecutar(request);
    Console.WriteLine($"[Éxito] Carátula modificada a: {request.NuevaCaratula}\n");
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


// ─── CAMINOS DE ERROR ─────────────────────────────────────────────────────────

Console.WriteLine("=== 6. Intentar crear un expediente con carátula vacía ===");
try
{
    var request = new AgregarExpedienteRequest("", idUsuario);
    agregarExpediente.Ejecutar(request);
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 7. Intentar operar con un expediente inexistente ===");
try
{
    var request = new CambiarEstadoRequest(Guid.NewGuid(), EstadoExpediente.Finalizado, idUsuario);
    cambiarEstado.Ejecutar(request);
}
catch (EntidadNoEncontradaException ex) { Console.WriteLine($"[EntidadNoEncontradaException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }

// Console.WriteLine("=== 8. Verificar AutorizacionException (cambiar AutorizacionProvisionalService a false) ===");
// if (autorizacionService.Autorizar){
//     Console.WriteLine("[Estado actual] Autorización habilitada.");
//     Console.WriteLine("Para probar AutorizacionException cambiar Autorizar a false.\n");
// }else{
//     Console.WriteLine("[Estado actual] Autorización deshabilitada.");
//     Console.WriteLine("Para volver al funcionamiento normal cambiar Autorizar a true.\n");
// }

Console.WriteLine("=== Fin de las pruebas ===");