using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Excepciones;
using SGE.Infraestructura.Repositorios;
using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;
using SGE.Infraestructura.Autorizacion;

Console.WriteLine("Iniciando Sistema de Gestión de Expedientes (SGE)...\n");

// 1. COMPOSITION ROOT (Armado de dependencias)
// A. Instanciamos la Infraestructura
var expedienteRepo = new ExpedienteTxtRepository();
var tramiteRepo = new TramiteTxtRepository();
var authService = new AutorizacionProvisionalService(); 
// B. Instanciamos los Servicios de Aplicación
var actualizadorEstado = new ActualizacionEstadoExpedienteService(expedienteRepo, tramiteRepo);
// C. Instanciamos los Casos de Uso (Inyectando las dependencias)
var agregarExpedienteUC = new AgregarExpedienteUseCase(expedienteRepo, authService);
var listarExpedientesUC = new ListarExpedientesUseCase(expedienteRepo);
var agregarTramiteUC = new AgregarTramiteUseCase(tramiteRepo, expedienteRepo, authService, actualizadorEstado);
var listarTramitesUC = new ListarTramitesPorExpedienteUseCase(tramiteRepo);

// Creamos un ID de usuario ficticio para simular quién usa el sistema
Guid idUsuarioActual = Guid.NewGuid(); 

// 2. SIMULACIÓN: EL CAMINO FELIZ
Console.WriteLine("--- PRUEBA 1: CAMINO FELIZ ---");
Guid idExpedienteCreado = Guid.Empty;

try
{
    // 2.1 Alta de Expediente
    var requestExpediente = new AgregarExpedienteRequest("Solicitud de beca universitaria", idUsuarioActual);
    var responseExpediente = agregarExpedienteUC.Ejecutar(requestExpediente);
    idExpedienteCreado = responseExpediente.IdExpediente;
    
    Console.WriteLine($"[ÉXITO] Expediente creado con ID: {idExpedienteCreado}");

    // 2.2 Agregamos un trámite que debería cambiar el estado a "ConResolucion"
    var requestTramite = new AgregarTramiteRequest(
        idExpedienteCreado, 
        EtiquetaTramite.Resolucion, 
        "Se aprueba la beca por buen desempeño", 
        idUsuarioActual);
        
    agregarTramiteUC.Ejecutar(requestTramite);
    Console.WriteLine($"[ÉXITO] Trámite de 'Resolución' agregado al expediente.");

    // 2.3 Listamos para verificar que el estado haya cambiado automáticamente
    Console.WriteLine("\nListado de Expedientes Actuales:");
    var expedientes = listarExpedientesUC.Ejecutar();
    foreach (var exp in expedientes)
    {
        Console.WriteLine($"- ID: {exp.Id} | Carátula: {exp.Caratula} | Estado: {exp.Estado}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error inesperado en el camino feliz: {ex.Message}");
}

Console.WriteLine("\nPresione ENTER para continuar con las pruebas de error...");
Console.ReadLine();

// 3. SIMULACIÓN: CAMINOS DE ERROR
Console.WriteLine("--- PRUEBA 2: CAMINOS DE ERROR ---");

// 3.1 Error de Dominio (Carátula vacía)
try
{
    Console.WriteLine("\nIntentando crear un expediente con carátula vacía...");
    var requestMalo = new AgregarExpedienteRequest("", idUsuarioActual);
    agregarExpedienteUC.Ejecutar(requestMalo);
}
catch (DominioException ex) // Atrapamos el error específico del dominio
{
    Console.WriteLine($"[DOMINIO]: {ex.Message}");
}

// 3.2 Error de Entidad No Encontrada
try
{
    Console.WriteLine("\nIntentando agregar un trámite a un expediente que no existe...");
    var requestMalo = new AgregarTramiteRequest(Guid.NewGuid(), EtiquetaTramite.PaseAEstudio, "Texto", idUsuarioActual);
    agregarTramiteUC.Ejecutar(requestMalo);
}
catch (EntidadNoEncontradaException ex) // Atrapamos nuestro error personalizado de aplicación
{
    Console.WriteLine($"[APLICACIÓN]: {ex.Message}");
}

// 3.3 Error de Autorización (Instrucción para tu prueba)
Console.WriteLine("\n[AUTORIZACIÓN]:");
Console.WriteLine("Para probar el error de permisos, andá al archivo 'ServicioAutorizacionProvisorio.cs', cambiá el 'return true;' por 'return false;' y volvé a ejecutar el programa. Debería saltar la AutorizacionException.");

Console.WriteLine("\nPruebas finalizadas con éxito! Revisá los archivos .txt en la carpeta bin/Debug/net10.0/");