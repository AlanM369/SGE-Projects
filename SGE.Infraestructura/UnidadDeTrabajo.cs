using SGE.Aplicacion.Comun;


//Guarda los cambios en la base de datos
namespace SGE.Infraestructura;

public class UnidadDeTrabajo(SgeContext context) : IUnidadDeTrabajo
{
    public void Guardar() => context.SaveChanges();
}