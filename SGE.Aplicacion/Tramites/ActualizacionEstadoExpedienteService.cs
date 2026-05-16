using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;

namespace SGE.Aplicacion.Tramites;

public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _expedienteRepository;
    private readonly ITramiteRepository _tramiteRepository;

    public ActualizacionEstadoExpedienteService(
        IExpedienteRepository expedienteRepository,
        ITramiteRepository tramiteRepository)
    {
        _expedienteRepository = expedienteRepository;
        _tramiteRepository = tramiteRepository;
    }

    public void Ejecutar(Guid expedienteId, Guid idUsuario)
    {
        // 1. Busca el expediente
        var expediente = _expedienteRepository.ObtenerPorId(expedienteId)
            ?? throw new EntidadNoEncontradaException("Expediente no encontrado.");

        // 2. Busca todos los trámites de ese expediente y agarra el último (más reciente)
        var tramites = _tramiteRepository.ObtenerPorExpedienteId(expedienteId);
        var ultimaEtiqueta = tramites
            .OrderByDescending(t => t.FechaCreacion)
            .FirstOrDefault()?.Etiqueta;

        // 3. Le pide a la entidad que evalúe si debe cambiar de estado
        // (toda la lógica vive en Expediente.ActualizarEstado, no acá)
        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, idUsuario);

        // 4. Solo persiste si realmente hubo un cambio
        if (cambio)
            _expedienteRepository.Modificar(expediente);
    }
}