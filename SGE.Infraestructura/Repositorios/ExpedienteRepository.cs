using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Persistencia;

namespace SGE.Infraestructura.Repositorios;

// Repositorio de infraestructura para la persistencia y gestión de expedientes utilizando Entity Framework Core
public class ExpedienteRepository(SgeContext context) : IExpedienteRepository
{
    public void Agregar(Expediente expediente)
    {
        context.Expedientes.Add(expediente);
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        return context.Expedientes
            .Where(e => e.Id == id)
            .SingleOrDefault();
    }

    public IEnumerable<Expediente> ObtenerTodos()
    {
        return context.Expedientes.ToList();
    }

    public void Modificar(Expediente expediente)
    {
        // EF Core trackea los cambios automáticamente, no es necesario llamar a Update()
    }

    public void Eliminar(Guid id)
    {
        var expediente = context.Expedientes
            .Where(e => e.Id == id)
            .SingleOrDefault();
        if (expediente != null)
            context.Expedientes.Remove(expediente);
    }
}