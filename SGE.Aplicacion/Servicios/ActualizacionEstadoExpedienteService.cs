using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Servicios;

public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _expedienteRepo;
    private readonly ITramiteRepository _tramiteRepo;

    public ActualizacionEstadoExpedienteService(IExpedienteRepository expedienteRepo, ITramiteRepository tramiteRepo)
    {
        _expedienteRepo = expedienteRepo;
        _tramiteRepo = tramiteRepo;
    }

    public void Ejecutar(Guid expedienteId, Guid idUsuario)
    {
        // 1. Buscamos el expediente
        var expediente = _expedienteRepo.ObtenerPorId(expedienteId);
        if (expediente == null) return; // Si no existe, no hacemos nada

        // 2. Buscamos todos sus trámites para ver cuál es el "último"
        var tramites = _tramiteRepo.ObtenerPorExpedienteId(expedienteId);
        
        // Obtenemos el trámite con la fecha de creación más reciente
        var ultimoTramite = tramites.OrderByDescending(t => t.FechaCreacion).FirstOrDefault();
        
        // Extraemos su etiqueta (si no hay trámites, enviamos null)
        EtiquetaTramite? ultimaEtiqueta = ultimoTramite?.Etiqueta;

        // 3. Le pedimos a la entidad que evalúe si debe cambiar su estado
        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, idUsuario);

        // 4. Si la entidad nos avisa que efectivamente mutó, guardamos el cambio
        if (cambio)
        {
            _expedienteRepo.Modificar(expediente);
        }
    }
}