using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase
{
    private readonly IExpedienteRepository _repositorio;
    private readonly IAutorizacionService _autorizacion;

    public CambiarEstadoExpedienteUseCase(IExpedienteRepository repo, IAutorizacionService auth)
    {
        _repositorio = repo;
        _autorizacion = auth;
    }

    public void Ejecutar(CambiarEstadoRequest request)
    {
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar el estado.");
        }

        var expediente = _repositorio.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // Modificamos el estado a través de la entidad
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        _repositorio.Modificar(expediente);
    }
}