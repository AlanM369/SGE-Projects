using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.Infraestructura;

// Implementación concreta de ITramiteRepository.
// Persiste los trámites en un archivo .txt, guardando cada dato en una línea separada.
public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombreArchivo = "tramites.txt";

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

    public Tramite? ObtenerPorId(Guid id)
    {
        return ObtenerTodos().FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return ObtenerTodos().Where(t => t.ExpedienteId == expedienteId);
    }

    public void Modificar(Tramite tramite)
    {
        var todos = ObtenerTodos().ToList();
        var indice = todos.FindIndex(t => t.Id == tramite.Id);

        if (indice == -1)
            throw new RepositorioException($"No se encontró el trámite con Id {tramite.Id} para modificar.");

        todos[indice] = tramite;
        ReescribirArchivo(todos);
    }

    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();
        var cantidad = todos.RemoveAll(t => t.Id == id);

        if (cantidad == 0)
            throw new RepositorioException($"No se encontró el trámite con Id {id} para eliminar.");

        ReescribirArchivo(todos);
    }

    public void EliminarPorExpedienteId(Guid expedienteId)
    {
        var todos = ObtenerTodos().ToList();
        todos.RemoveAll(t => t.ExpedienteId == expedienteId);
        ReescribirArchivo(todos);
    }

    private IEnumerable<Tramite> ObtenerTodos()
    {
        var resultado = new List<Tramite>();

        if (!File.Exists(_nombreArchivo)) return resultado;

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

            var id          = Guid.Parse(idStr);
            var expedienteId = Guid.Parse(expedienteIdStr);
            var etiqueta    = Enum.Parse<EtiquetaTramite>(etiquetaStr);
            var contenido   = new ContenidoTramite(contenidoStr);
            var fechaCre    = DateTime.Parse(fechaCreStr);
            var fechaMod    = DateTime.Parse(fechaModStr);
            var usuario     = Guid.Parse(usuarioStr);

            var tramite = Tramite.Reconstruir(id, expedienteId, etiqueta, contenido, fechaCre, fechaMod, usuario);
            resultado.Add(tramite);
        }

        return resultado;
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
            sw.WriteLine(t.Contenido!.Texto);
            sw.WriteLine(t.FechaCreacion.ToString("O"));
            sw.WriteLine(t.FechaUltimaModificacion.ToString("O"));
            sw.WriteLine(t.UsuarioUltimoCambio);
        }
    }
}