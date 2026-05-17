using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class ListarExpedientesUseCase(IExpedienteRepository repositorio)
{
    // El método devuelve un IEnumerable (una secuencia) de DTOs, no de Entidades
    public IEnumerable<ExpedienteDetalleDTOs> Ejecutar()
    {   
        // 1. Obtenemos TODAS las entidades crudas desde la infraestructura
        var expedientes = repositorio.ObtenerTodos();
        // Creamos una lista vacía para guardar nuestros DTOs
        var listaDinamica = new List<ExpedienteDetalleDTOs>();
        // 2. Proceso de Mapeo
        foreach (var exp in expedientes)
        {
            /// Convertimos la Entidad de Dominio en un DTO
            var dto = new ExpedienteDetalleDTOs(
                exp.Id, 
                exp.Caratula.Texto, // extraemos el texto del Value Object
                exp.Estado, 
                exp.FechaCreacion, 
                exp.FechaUltimaModificacion
            );

            // Agregamos el DTO a nuestra lista final
            listaDinamica.Add(dto);
        }
        // 3. Devolvemos la lista segura
        return listaDinamica;
    }
}