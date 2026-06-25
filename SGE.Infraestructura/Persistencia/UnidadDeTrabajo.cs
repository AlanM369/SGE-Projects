using SGE.Aplicacion.Comun;


// Confirmar y persistir de manera atómica en la base de datos
namespace SGE.Infraestructura.Persistencia;

public class UnidadDeTrabajo(SgeContext context) : IUnidadDeTrabajo
{
    public void Guardar() => context.SaveChanges();
}