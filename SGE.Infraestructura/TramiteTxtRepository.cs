using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;

namespace SGE.Infraestructura;

public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombreArchivo = "tramites.txt";

    public TramiteTxtRepository()
    {
        // Aseguramos que el archivo exista antes de intentar leerlo
        if (!File.Exists(_nombreArchivo))
            File.Create(_nombreArchivo).Close();
    }

    public void Agregar(Tramite tramite)
    {
        using var sw = new StreamWriter(_nombreArchivo, true);
        sw.WriteLine(tramite.Id);
        sw.WriteLine(tramite.ExpedienteId);
        sw.WriteLine(tramite.Etiqueta);
        sw.WriteLine(tramite.Contenido!.Texto);
        sw.WriteLine(tramite.FechaCreacion.ToString("O"));
        sw.WriteLine(tramite.FechaUltimaModificacion.ToString("O"));
        sw.WriteLine(tramite.UsuarioUltimoCambio);
    }

    // Método auxiliar privado (o público si querés) para traer todos y facilitar la búsqueda
    private IEnumerable<Tramite> ObtenerTodos()
    {
        var resultado = new List<Tramite>();

        // Si el archivo no existe devolvemos la lista vacía
        if (!File.Exists(_nombreArchivo)) 
            return resultado;

        using var sr = new StreamReader(_nombreArchivo);
        while (!sr.EndOfStream)
        {   
            var linea = sr.ReadLine();
            // si la línea está vacía o es puro espacio, la ignoramos y salimos del ciclo
            if (string.IsNullOrWhiteSpace(linea)) 
                break;
            // 1. Leemos los datos del TXT (7 líneas por cada tramite)
            var idStr = linea;
            var expedienteIdStr = sr.ReadLine() ?? "";
            var etiquetaStr = sr.ReadLine() ?? "";
            var contenidoStr = sr.ReadLine() ?? "";
            var fechaCreStr = sr.ReadLine() ?? "";
            var fechaModStr = sr.ReadLine() ?? "";
            var usuarioStr = sr.ReadLine() ?? "";

            // 2. Recreamos los Value Objects (aquí habrá validación)
            var id = Guid.Parse(idStr);
            var expedienteId = Guid.Parse(expedienteIdStr);
            var etiqueta = Enum.Parse<EtiquetaTramite>(etiquetaStr);
            var contenido = new ContenidoTramite(contenidoStr);
            var fechaCre = DateTime.Parse(fechaCreStr);
            var fechaMod = DateTime.Parse(fechaModStr);
            var usuario = Guid.Parse(usuarioStr);

            // 3. Reconstruimos la Entidad usando el Factory Method
            var tramite = Tramite.Reconstruir(id, expedienteId, etiqueta, contenido, fechaCre, fechaMod, usuario);
            resultado.Add(tramite);
        }
        

        return resultado;
    }

    // Cumplimos el contrato de la interfaz filtrando la lista completa
    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return ObtenerTodos().Where(t => t.ExpedienteId == expedienteId);
    }

    public Tramite? ObtenerPorId(Guid id)
    {   
        //recorre la lista hasta encontrar el trámite
        return ObtenerTodos().FirstOrDefault(t => t.Id == id);
    }

    public void Modificar(Tramite tramite)
    {
        var todos = ObtenerTodos().ToList();
        var index = todos.FindIndex(t => t.Id == tramite.Id);

        if (index == -1)
            throw new RepositorioException($"No se pudo modificar: El trámite {tramite.Id} no existe en la base de datos.");

        todos[index] = tramite;
        ReescribirArchivo(todos);
    }

    //recorre la lista una sola vez. Si encuentra elementos que coinciden con la condición, los elimina directamente
    //y te devuelve un número (cantidad) diciendo cuántos borró.
    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();
        var cantidad = todos.RemoveAll(t => t.Id == id);

        if (cantidad == 0)
            throw new RepositorioException($"No se pudo eliminar: El trámite {id} no existe en la base de datos.");

        ReescribirArchivo(todos);
            
    }

    public void EliminarPorExpedienteId(Guid expedienteId)
    {
        var todos = ObtenerTodos().ToList();
        todos.RemoveAll(t => t.ExpedienteId == expedienteId);
        ReescribirArchivo(todos);
    }

    // Lee todo, modifica en memoria y reescribe el archivo completo
    private void ReescribirArchivo(List<Tramite> tramites)
    {
        using var sw = new StreamWriter(_nombreArchivo, false);
        foreach (var t in tramites)
        {
            sw.WriteLine(t.Id);
            sw.WriteLine(t.ExpedienteId);
            sw.WriteLine(t.Etiqueta);
            sw.WriteLine(t.Contenido);
            sw.WriteLine(t.FechaCreacion.ToString("O"));
            sw.WriteLine(t.FechaUltimaModificacion.ToString("O"));
            sw.WriteLine(t.UsuarioUltimoCambio);
        }
    }
}