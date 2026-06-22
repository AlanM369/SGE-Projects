using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.Infraestructura.Repositorios;

public class TramiteRepository(SgeContext context) : ITramiteRepository
{
    public void Agregar(Tramite tramite)
    {
        context.Tramites.Add(tramite);
    }

    public Tramite? ObtenerPorId(Guid id)
    {
        return context.Tramites
            .Where(t => t.Id == id)
            .SingleOrDefault();
    }

    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return context.Tramites
            .Where(t => t.ExpedienteId == expedienteId)
            .ToList();
    }

    //Lo mismo que expediente con el modificar
    public void Modificar(Tramite tramite)
    {
        // EF Core trackea los cambios automáticamente,
        // no es necesario llamar a Update()
    }

    public void Eliminar(Guid id)
    {
        var tramite = context.Tramites
            .Where(t => t.Id == id)
            .SingleOrDefault();
        if (tramite != null)
            context.Tramites.Remove(tramite);
    }
}