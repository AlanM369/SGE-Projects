using System.Linq;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase
{
    private readonly IExpedienteRepository _repo;
    private readonly IAutorizacionService _auth;

    public ModificarCaratulaExpedienteUseCase(IExpedienteRepository repo, IAutorizacionService auth)
    {
        _repo = repo;
        _auth = auth;
    }

    public void Ejecutar(ModificarCaratulaRequest request)
    {
        if (!_auth.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar expedientes.");
        }

        var expediente = _repo.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        var nuevaCaratulaVO = new Caratula(request.NuevaCaratula);
        
        // Le pasamos la responsabilidad a la entidad
        expediente.ModificarCaratula(nuevaCaratulaVO, request.IdUsuario);

        _repo.Modificar(expediente);
    }
}