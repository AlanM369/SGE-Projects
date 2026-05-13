using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase
{
    private readonly IExpedienteRepository _repo;
    private readonly IAutorizacionService _auth;

    public CambiarEstadoExpedienteUseCase(IExpedienteRepository repo, IAutorizacionService auth)
    {
        _repo = repo;
        _auth = auth;
    }

    public void Ejecutar(CambiarEstadoRequest request)
    {
        if (!_auth.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar el estado.");
        }

        var expediente = _repo.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // Modificamos el estado a través de la entidad
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        _repo.Modificar(expediente);
    }
}