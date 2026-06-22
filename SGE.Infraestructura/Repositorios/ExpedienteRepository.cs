using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;

namespace SGE.Infraestructura.Repositorios;

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
        // EF Core trackea los cambios automáticamente,
        // no es necesario llamar a Update()
    }

    //En la teoria el modificar es automatico como arriba, pero esto puede traer problemas si el objeto no esta traido de la base de datos, cuidado
    // public void Modificar(Expediente expediente)
    // {
    //     context.Expedientes.Update(expediente);
    // }

    public void Eliminar(Guid id)
    {
        var expediente = context.Expedientes
            .Where(e => e.Id == id)
            .SingleOrDefault();
        if (expediente != null)
            context.Expedientes.Remove(expediente);
    }
}