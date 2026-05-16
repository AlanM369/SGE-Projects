using SGE.Aplicacion.Interfaces;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Excepciones;

namespace SGE.Infraestructura.Repositorios;

public class ExpedienteTxtRepository : IExpedienteRepository
{
    private readonly string _rutaArchivo = "expedientes.txt";

    public ExpedienteTxtRepository()
    {
        // Si el archivo no existe, lo creamos vacío
        if (!File.Exists(_rutaArchivo))
        {
            File.Create(_rutaArchivo).Close();
        }
    }

    public void Agregar(Expediente expediente)
    {
        // Extraemos los datos de la entidad y de sus Value Objects
        string linea = $"{expediente.Id},{expediente.Caratula.Texto},{(int)expediente.Estado},{expediente.FechaCreacion:O},{expediente.FechaUltimaModificacion:O},{expediente.UsuarioUltimoCambio}";
        
        // AppendAllText agrega la línea al final del archivo
        File.AppendAllText(_rutaArchivo, linea + Environment.NewLine);
    }

    public IEnumerable<Expediente> ObtenerTodos()
    {
        var expedientes = new List<Expediente>();
        var lineas = File.ReadAllLines(_rutaArchivo);

        foreach (var linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            var datos = linea.Split(',');

            // Parseamos los datos del string hacia sus tipos reales
            var id = Guid.Parse(datos[0]);
            var caratula = new Caratula(datos[1]);
            var estado = (EstadoExpediente)int.Parse(datos[2]);
            var fechaCreacion = DateTime.Parse(datos[3]);
            var fechaModificacion = DateTime.Parse(datos[4]);
            var usuarioId = Guid.Parse(datos[5]);

            // ¡Acá usamos el Factory Method que nos pidió el TP!
            var expediente = Expediente.Reconstruir(id, caratula, fechaCreacion, fechaModificacion, usuarioId, estado);
            
            expedientes.Add(expediente);
        }

        return expedientes;
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        // Reutilizamos ObtenerTodos y buscamos el que coincida
        return ObtenerTodos().FirstOrDefault(e => e.Id == id);
    }

    public void Modificar(Expediente expediente)
    {
        var todos = ObtenerTodos().ToList();
        var index = todos.FindIndex(e => e.Id == expediente.Id);

        if (index != -1)
        {
            todos[index] = expediente; // Reemplazamos la entidad vieja por la actualizada
            ReescribirArchivo(todos);  // Guardamos todo de nuevo
        }
        else
        {
            throw new RepositorioException($"No se pudo modificar: El expediente {expediente.Id} no existe en la base de datos.");
        }
    }

    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();
        var expediente = todos.FirstOrDefault(e => e.Id == id);

        if (expediente != null)
        {
            todos.Remove(expediente); // Borramos el elemento de la lista
            ReescribirArchivo(todos); // Sobreescribimos el archivo sin ese elemento
        }
        else
        {
            throw new RepositorioException($"No se pudo eliminar: El expediente {id} no existe en la base de datos.");
        }
    }

    // Método auxiliar privado para no repetir código al guardar
    private void ReescribirArchivo(List<Expediente> expedientes)
    {
        File.WriteAllText(_rutaArchivo, string.Empty); // Borramos el contenido actual
        foreach (var exp in expedientes)
        {
            Agregar(exp); // Reutilizamos el método Agregar para escribir línea por línea
        }
    }
}