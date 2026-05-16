using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Tramites;
using System.IO;

namespace SGE.Infraestructura.Repositorios;

public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _rutaArchivo = "tramites.txt";

    public TramiteTxtRepository()
    {
        // Aseguramos que el archivo exista antes de intentar leerlo
        if (!File.Exists(_rutaArchivo))
        {
            File.Create(_rutaArchivo).Close();
        }
    }

    public void Agregar(Tramite tramite)
    {
        // Guardamos los datos separados por coma.
        // Ojo acá: accedemos a tramite.Contenido.Texto para sacar el string real
        string linea = $"{tramite.Id},{tramite.ExpedienteId},{(int)tramite.Etiqueta},{tramite.Contenido.Texto},{tramite.FechaCreacion:O},{tramite.FechaUltimaModificacion:O},{tramite.UsuarioUltimoCambio}";
        
        File.AppendAllText(_rutaArchivo, linea + Environment.NewLine);
    }

    // Método auxiliar privado (o público si querés) para traer todos y facilitar la búsqueda
    private IEnumerable<Tramite> ObtenerTodos()
    {
        var tramites = new List<Tramite>();
        var lineas = File.ReadAllLines(_rutaArchivo);

        foreach (var linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            var datos = linea.Split(',');

            var id = Guid.Parse(datos[0]);
            var expedienteId = Guid.Parse(datos[1]);
            var etiqueta = (EtiquetaTramite)int.Parse(datos[2]);
            var contenido = new ContenidoTramite(datos[3]); // Reconstruimos el Value Object
            var fechaCreacion = DateTime.Parse(datos[4]);
            var fechaModificacion = DateTime.Parse(datos[5]);
            var usuarioId = Guid.Parse(datos[6]);

            // Reconstruimos la entidad usando el Factory Method
            var tramite = Tramite.Reconstruir(id, expedienteId, etiqueta, contenido, fechaCreacion, fechaModificacion, usuarioId);
            
            tramites.Add(tramite);
        }

        return tramites;
    }

    // Cumplimos el contrato de la interfaz filtrando la lista completa
    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return ObtenerTodos().Where(t => t.ExpedienteId == expedienteId);
    }

    public Tramite? ObtenerPorId(Guid id)
    {
        return ObtenerTodos().FirstOrDefault(t => t.Id == id);
    }

    public void Modificar(Tramite tramite)
    {
        var todos = ObtenerTodos().ToList();
        var index = todos.FindIndex(t => t.Id == tramite.Id);

        if (index != -1)
        {
            todos[index] = tramite;
            ReescribirArchivo(todos);
        }
        else
        {
            throw new Exception($"No se pudo modificar: El trámite {tramite.Id} no existe en la base de datos.");
        }
    }

    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();
        var tramite = todos.FirstOrDefault(t => t.Id == id);

        if (tramite != null)
        {
            todos.Remove(tramite);
            ReescribirArchivo(todos);
        }
        else
        {
            throw new Exception($"No se pudo eliminar: El trámite {id} no existe en la base de datos.");
        }
    }

    private void ReescribirArchivo(List<Tramite> tramites)
    {
        File.WriteAllText(_rutaArchivo, string.Empty);
        foreach (var t in tramites)
        {
            Agregar(t);
        }
    }
}