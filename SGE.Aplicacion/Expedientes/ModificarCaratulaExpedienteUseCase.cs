using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase
{
    private readonly IExpedienteRepository _repositorio;
    private readonly IAutorizacionService _autorizacion;

    public ModificarCaratulaExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion)
    {
        _repositorio = repositorio;
        _autorizacion = autorizacion;
    }

    public void Ejecutar(ModificarCaratulaRequest request)
    {   
        // 1. Se realiza el cheque de autorización
        if (!_autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar expedientes.");
        }

        // 2. Búsqueda y control de nulos (Acá usamos nuestra excepción personalizada)
        var expediente = _repositorio.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // 3. Dominio: Creamos el Value Object y le pedimos a la entidad que se actualice
        var nuevaCaratulaVO = new Caratula(request.NuevaCaratula);
        expediente.ModificarCaratula(nuevaCaratulaVO, request.IdUsuario);

        // 4. Persistencia: Guardamos el estado modificado
        _repositorio.Modificar(expediente);
    }
}