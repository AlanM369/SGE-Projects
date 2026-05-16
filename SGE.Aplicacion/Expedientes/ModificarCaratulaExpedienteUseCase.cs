using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Excepciones;
using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion)
{
    public void Ejecutar(ModificarCaratulaRequest request)
    {   
        // 1. Se realiza el cheque de autorización
        if (!autorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar expedientes.");
        }

        // 2. Búsqueda y control de nulos (Acá usamos nuestra excepción personalizada)
        var expediente = repositorio.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // 3. Dominio: Creamos el Value Object y le pedimos a la entidad que se actualice
        var nuevaCaratulaVO = new Caratula(request.NuevaCaratula);
        expediente.ModificarCaratula(nuevaCaratulaVO, request.IdUsuario);

        // 4. Persistencia: Guardamos el estado modificado
        repositorio.Modificar(expediente);
    }
}