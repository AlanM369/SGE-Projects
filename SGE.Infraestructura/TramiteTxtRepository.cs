using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.Infraestructura;

// Implementación concreta de ITramiteRepository.
// Persiste los trámites en un archivo .txt, guardando cada dato en una línea separada.
public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombreArchivo = "tramites.txt";

    // Agrega un nuevo trámite al archivo.
    public void Agregar(Tramite tramite)
    {
        // Guardamos cada dato en una línea nueva.
        using var sw = new StreamWriter(_nombreArchivo, true);
        sw.WriteLine(tramite.Id);
        sw.WriteLine(tramite.ExpedienteId);
        sw.WriteLine(tramite.Etiqueta);
        sw.WriteLine(tramite.Contenido!.Texto);
        sw.WriteLine(tramite.FechaCreacion.ToString("O"));
        sw.WriteLine(tramite.FechaUltimaModificacion.ToString("O"));
        sw.WriteLine(tramite.UsuarioUltimoCambio);
    }

    // Busca un trámite por su id. Lee el archivo línea por línea y devuelve el que coincide, o null si no se encuentra.
    public Tramite? ObtenerPorId(Guid id)
    {
        if (!File.Exists(_nombreArchivo)) return null;

        using var sr = new StreamReader(_nombreArchivo);
        while (!sr.EndOfStream)
        {
            var idStr           = sr.ReadLine() ?? "";
            var expedienteIdStr = sr.ReadLine() ?? "";
            var etiquetaStr     = sr.ReadLine() ?? "";
            var contenidoStr    = sr.ReadLine() ?? "";
            var fechaCreStr     = sr.ReadLine() ?? "";
            var fechaModStr     = sr.ReadLine() ?? "";
            var usuarioStr      = sr.ReadLine() ?? "";

            var tramiteId = Guid.Parse(idStr);

            // Si el Id coincide, reconstruye y devuelve ese trámite
            if (tramiteId == id)
            {
                return Tramite.Reconstruir(
                    tramiteId,
                    Guid.Parse(expedienteIdStr),
                    Enum.Parse<EtiquetaTramite>(etiquetaStr),
                    new ContenidoTramite(contenidoStr),
                    DateTime.Parse(fechaCreStr),
                    DateTime.Parse(fechaModStr),
                    Guid.Parse(usuarioStr));
            }
        }

        // Si recorrió todo el archivo y no lo encontró, devuelve null
        return null;
    }

    // Obtiene todos los trámites de un expediente dado su id.
    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return ObtenerTodos().Where(t => t.ExpedienteId == expedienteId);
    }

    // Obtiene todos los trámites del archivo.
    private IEnumerable<Tramite> ObtenerTodos()
    {
        var resultado = new List<Tramite>();

        if (!File.Exists(_nombreArchivo)) return resultado;

        using var sr = new StreamReader(_nombreArchivo);
        while (!sr.EndOfStream)
        {
            // Leemos los datos del TXT
            var idStr           = sr.ReadLine() ?? "";
            var expedienteIdStr = sr.ReadLine() ?? "";
            var etiquetaStr     = sr.ReadLine() ?? "";
            var contenidoStr    = sr.ReadLine() ?? "";
            var fechaCreStr     = sr.ReadLine() ?? "";
            var fechaModStr     = sr.ReadLine() ?? "";
            var usuarioStr      = sr.ReadLine() ?? "";

            // Recreamos los Value Objects
            var id           = Guid.Parse(idStr);
            var expedienteId = Guid.Parse(expedienteIdStr);
            var etiqueta     = Enum.Parse<EtiquetaTramite>(etiquetaStr);
            var contenido    = new ContenidoTramite(contenidoStr);
            var fechaCre     = DateTime.Parse(fechaCreStr);
            var fechaMod     = DateTime.Parse(fechaModStr);
            var usuario      = Guid.Parse(usuarioStr);

            // Reconstruimos la Entidad usando el Factory Method
            var tramite = Tramite.Reconstruir(id, expedienteId, etiqueta, contenido, fechaCre, fechaMod, usuario);
            resultado.Add(tramite);
        }

        return resultado;
    }

    // Modifica un trámite existente. Lee todos los trámites, busca el que coincide por id, lo reemplaza y reescribe todo el archivo.
    public void Modificar(Tramite tramite)
    {
        var todos = ObtenerTodos().ToList(); // Leemos todos los trámites del archivo a memoria para poder modificar.
        var indice = todos.FindIndex(t => t.Id == tramite.Id); // Buscamos el índice del trámite a modificar en la lista completa.

        // Si no lo encuentra, lanzamos una excepción.
        if (indice == -1)
            throw new RepositorioException($"No se encontró el trámite con Id {tramite.Id} para modificar.");

        todos[indice] = tramite; // Reemplazamos el trámite viejo por el trámite nuevo.
        ReescribirArchivo(todos); // Reescribimos el archivo completo con la lista modificada.
    }

    // Elimina un trámite por su id. Lee todos los trámites, elimina el que coincide por id, y reescribe todo el archivo.
    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList(); // Leemos todos los trámites del archivo a memoria para poder eliminar.
        var cantidad = todos.RemoveAll(t => t.Id == id); // Eliminamos el trámite que coincide con el id.

        // Si no se encuentra, lanzamos una excepción.
        if (cantidad == 0)
            throw new RepositorioException($"No se encontró el trámite con Id {id} para eliminar.");

        ReescribirArchivo(todos); // Reescribimos el archivo completo con la lista modificada.
    }

    // Elimina todos los trámites de un expediente. Usado por EliminarExpedienteUseCase para la baja en cascada.
    public void EliminarPorExpedienteId(Guid expedienteId)
    {
        var todos = ObtenerTodos().ToList(); // Leemos todos los trámites del archivo a memoria.
        todos.RemoveAll(t => t.ExpedienteId == expedienteId); // Eliminamos todos los trámites del expediente.
        ReescribirArchivo(todos); // Reescribimos el archivo completo con la lista modificada.
    }

    // Reescribe el archivo completo con la lista modificada.
    private void ReescribirArchivo(List<Tramite> tramites)
    {
        using var sw = new StreamWriter(_nombreArchivo, false);
        foreach (var t in tramites)
        {
            sw.WriteLine(t.Id);
            sw.WriteLine(t.ExpedienteId);
            sw.WriteLine(t.Etiqueta);
            sw.WriteLine(t.Contenido!.Texto);
            sw.WriteLine(t.FechaCreacion.ToString("O"));
            sw.WriteLine(t.FechaUltimaModificacion.ToString("O"));
            sw.WriteLine(t.UsuarioUltimoCambio);
        }
    }
}