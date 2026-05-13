using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository _repo;
    private readonly IAutorizacionService _auth;

    public AgregarExpedienteUseCase(IExpedienteRepository repo, IAutorizacionService auth)
    {
        _repo = repo;
        _auth = auth;
    }

    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        // 1. Verificamos permisos
        if (!_auth.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
        {
            throw new AutorizacionException("El usuario no tiene permisos para crear expedientes.");
        }

        // 2. Instanciamos el Value Object (validará que la carátula sea correcta) y la Entidad
        var caratulaVO = new Caratula(request.Caratula);
        var nuevoExpediente = new Expediente(caratulaVO, request.IdUsuario);

        // 3. Persistimos
        _repo.Agregar(nuevoExpediente);

        // 4. Retornamos la respuesta
        return new AgregarExpedienteResponse(nuevoExpediente.Id);
    }
}