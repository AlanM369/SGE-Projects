namespace SGE.Aplicacion.Expedientes;

public class ListarExpedientesUseCase
{
    private readonly IExpedienteRepository _expedienteRepository;

    public ListarExpedientesUseCase(IExpedienteRepository expedienteRepository)
    {
        _expedienteRepository = expedienteRepository;
    }

    // Listar no requiere autorización según el TP
    public IEnumerable<ListarExpedientesResponse> Ejecutar()
    {
        return _expedienteRepository.ObtenerTodos()
            .Select(e => new ListarExpedientesResponse(
                e.Id,
                e.Caratula!.Texto,
                e.Estado.ToString(),
                e.FechaCreacion,
                e.FechaUltimaModificacion));
    }
}