using SGE.Aplicacion.Autorizacion;// Para el Enum Permiso
using SGE.Aplicacion.Excepciones; // Para nuestra AutorizacionException
using SGE.Aplicacion.Interfaces; // Para los contratos
using SGE.Dominio.Expedientes; // Para las entidades

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase
{   
    // Usamos readonly para asegurar que estas dependencias no se modifiquen a mitad del proceso
    private readonly IExpedienteRepository _repositorio;
    private readonly IAutorizacionService _autorizacion;

    // INYECCIÓN DE DEPENDENCIAS
    public AgregarExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion)
    {
        _repositorio = repositorio;
        _autorizacion = autorizacion;
    }

    // El método Ejecutar recibe el DTO de entrada y devuelve el DTO de salida
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        // 1. Verificamos permisos
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
        {
            throw new AutorizacionException("El usuario no tiene permisos para crear expedientes.");
        }

        // 2. Interacción con el Dominio 
        // Instanciamos el Value Object (validará que la carátula sea correcta) y la Entidad
        var caratulaVO = new Caratula(request.Caratula);
        var nuevoExpediente = new Expediente(caratulaVO, request.IdUsuario);

        // 3. Persistencia
        // Le pasamos la entidad al repositorio para que la guarde
        _repositorio.Agregar(nuevoExpediente);

        // 4. Retornamos la respuesta
        // Empaquetamos el resultado en un DTO y se lo mandamos a la capa que nos llamó (que seria la Consola, aunque no lo sabe)
        return new AgregarExpedienteResponse(nuevoExpediente.Id);
    }
}