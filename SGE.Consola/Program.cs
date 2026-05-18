using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Infraestructura;


// ─── COMPOSITION ROOT ────────────────────────────────────────────────────────
// Único lugar en todo el sistema donde se crean instancias concretas.
// Los casos de uso solo conocen interfaces, nunca estas clases concretas.

// Infraestructura
var expedienteRepository    = new ExpedienteTxtRepository();
var tramiteRepository       = new TramiteTxtRepository();
var autorizacionService     = new AutorizacionProvisionalService();
var actualizarEstado = new ActualizacionEstadoExpedienteService(expedienteRepository, tramiteRepository);

// Casos de uso de Expedientes
var agregarExpediente       = new AgregarExpedienteUseCase(expedienteRepository, autorizacionService);
var eliminarExpediente      = new BajaExpedienteUseCase(expedienteRepository, tramiteRepository, autorizacionService);
var modificarCaratula       = new ModificarCaratulaExpedienteUseCase(expedienteRepository, autorizacionService);
var cambiarEstado           = new CambiarEstadoExpedienteUseCase(expedienteRepository, autorizacionService);
var listarExpedientes       = new ListarExpedientesUseCase(expedienteRepository);

// Casos de uso de Trámites (completar con los de tu compañero)
var agregarTramite          = new AgregarTramiteUseCase(tramiteRepository, expedienteRepository, autorizacionService, actualizarEstado);
var eliminarTramite         = new BajaTramiteUseCase(tramiteRepository,autorizacionService, actualizarEstado);
var modificarTramite        = new ModificarTramiteUseCase(tramiteRepository, autorizacionService, actualizarEstado);
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
    var expedientes = listarExpedientes.Ejecutar();
    foreach (var e in expedientes)
        Console.WriteLine($"> Id: {e.Id} | Carátula: {e.Caratula} | Estado: {e.Estado}");
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 3. Agregar un trámite y verificar cambio de estado automático ===");
try
{
    // Primero obtenemos el id del expediente que creamos antes
    var expedientes = listarExpedientes.Ejecutar().ToList();
    var idExpediente = expedientes.First().Id;

    // Agregamos un trámite con etiqueta PaseAEstudio → debe cambiar el estado a ParaResolver
    var request = new AgregarTramiteRequest(idExpediente, EtiquetaTramite.PaseAEstudio, "Pase a estudio del expediente", idUsuario);
    var response = agregarTramite.Ejecutar(request);
    Console.WriteLine($"[Éxito] Trámite agregado. Id: {response.IdTramite} | Etiqueta: {request.Etiqueta}");

    // Verificamos que el estado del expediente cambió automáticamente
    var expedienteActualizado = listarExpedientes.Ejecutar().First(e => e.Id == idExpediente);
    Console.WriteLine($"[Verificación] Estado del expediente: {expedienteActualizado.Estado} (esperado: ParaResolver)\n");
}
catch (DominioException ex) { Console.WriteLine($"[DominioException]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[AutorizacionException]: {ex.Message}\n"); }
catch (Exception ex) { Console.WriteLine($"[Exception]: {ex.Message}\n"); }


Console.WriteLine("=== 4. Cambiar estado manualmente ===");
try
{
    var idExpediente = listarExpedientes.Ejecutar().First().Id;
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
    var idExpediente = listarExpedientes.Ejecutar().First().Id;
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

Console.WriteLine("=== 8. Verificar AutorizacionException (cambiar AutorizacionProvisionalService a false) ===");
if (autorizacionService.Autorizar){
    Console.WriteLine("[Estado actual] Autorización habilitada.");
    Console.WriteLine("Para probar AutorizacionException cambiar Autorizar a false.\n");
}else{
    Console.WriteLine("[Estado actual] Autorización deshabilitada.");
    Console.WriteLine("Para volver al funcionamiento normal cambiar Autorizar a true.\n");
}

Console.WriteLine("=== Fin de las pruebas ===");