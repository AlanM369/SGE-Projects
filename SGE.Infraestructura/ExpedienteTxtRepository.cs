using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Comun;

namespace SGE.Infraestructura;

// Implementación concreta de IExpedienteRepository.
// Persiste los expedientes en un archivo .txt, guardando cada dato en una línea separada.
public class ExpedienteTxtRepository : IExpedienteRepository
{
    private readonly string _nombreArchivo = "expedientes.txt";

    // Agrega un nuevo expediente al archivo. 
    public void Agregar(Expediente expediente)
    {
         // Guardamos cada dato en una línea nueva. 
        using var sw = new StreamWriter(_nombreArchivo, true);
        sw.WriteLine(expediente.Id);
        sw.WriteLine(expediente.Caratula!.Texto);
        sw.WriteLine(expediente.FechaCreacion.ToString("O"));
        sw.WriteLine(expediente.FechaUltimaModificacion.ToString("O"));
        sw.WriteLine(expediente.UsuarioUltimoCambio);
        sw.WriteLine(expediente.Estado);
    }

    // Busca un expediente por su id. Lee todos los expedientes del archivo y devuelve el que coincide con el id, o null si no se encuentra.
    public Expediente? ObtenerPorId(Guid id)
    {
        if (!File.Exists(_nombreArchivo)) return null;

            using var sr = new StreamReader(_nombreArchivo);
            while (!sr.EndOfStream)
            {
                var idStr        = sr.ReadLine() ?? "";
                var caratulaStr  = sr.ReadLine() ?? "";
                var fechaCreStr  = sr.ReadLine() ?? "";
                var fechaModStr  = sr.ReadLine() ?? "";
                var usuarioStr   = sr.ReadLine() ?? "";
                var estadoStr    = sr.ReadLine() ?? "";

                var expedienteId = Guid.Parse(idStr);

                // Si el Id coincide, reconstruye y devuelve ese expediente
                if (expedienteId == id)
                {
                    return Expediente.Reconstruir(
                        expedienteId,
                        new Caratula(caratulaStr),
                        DateTime.Parse(fechaCreStr),
                        DateTime.Parse(fechaModStr),
                        Guid.Parse(usuarioStr),
                        Enum.Parse<EstadoExpediente>(estadoStr));
                }
    }

    // Si recorrió todo el archivo y no lo encontró, devuelve null
    return null;
}

    // Obtiene todos los expedientes del archivo.
    public IEnumerable<Expediente> ObtenerTodos()
    {
        var resultado = new List<Expediente>();

        if (!File.Exists(_nombreArchivo)) return resultado;

        using var sr = new StreamReader(_nombreArchivo);
        while (!sr.EndOfStream)
        {
            // Leemos los datos del TXT
            var idStr           = sr.ReadLine() ?? "";
            var caratulaStr     = sr.ReadLine() ?? "";
            var fechaCreStr     = sr.ReadLine() ?? "";
            var fechaModStr     = sr.ReadLine() ?? "";
            var usuarioStr      = sr.ReadLine() ?? "";
            var estadoStr       = sr.ReadLine() ?? "";

            // Recreamos los Value Objects 
            var id          = Guid.Parse(idStr);
            var caratula    = new Caratula(caratulaStr);
            var fechaCre    = DateTime.Parse(fechaCreStr);
            var fechaMod    = DateTime.Parse(fechaModStr);
            var usuario     = Guid.Parse(usuarioStr);
            var estado      = Enum.Parse<EstadoExpediente>(estadoStr);

            // Reconstruimos la Entidad usando el Factory Method
            var expediente = Expediente.Reconstruir(id, caratula, fechaCre, fechaMod, usuario, estado);
            resultado.Add(expediente);
        }

        return resultado;
    }

    // Modifica un expediente existente. Lee todos los expedientes, busca el que coincide por id, lo reemplaza y reescribe todo el archivo.
    public void Modificar(Expediente expediente)
    {
        var todos = ObtenerTodos().ToList(); // Leemos todos los expedientes del archivo a memoria para poder modificar.
        var indice = todos.FindIndex(e => e.Id == expediente.Id); // Buscamos el índice del expediente a modificar en la lista completa

        // Si no lo encuentra, lanzamos una excepción.
        if (indice == -1)
            throw new RepositorioException($"No se encontró el expediente con Id {expediente.Id} para modificar.");

        todos[indice] = expediente; // Reemplazamos el expediente viejo por el expediente nuevo.
        ReescribirArchivo(todos); // Reescribimos el archivo completo con la lista modificada. 
    }

    // Elimina un expediente por su id. Lee todos los expedientes, elimina el que coincide por id, y reescribe todo el archivo.
    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();  // Leemos todos los expedientes del archivo a memoria para poder modificar.
        var cantidad = todos.RemoveAll(e => e.Id == id); // Buscamos el índice del expediente a modificar en la lista completa

        // Si no se encuentra, lanzamos una excepción.
        if (cantidad == 0)
            throw new RepositorioException($"No se encontró el expediente con Id {id} para eliminar.");

        ReescribirArchivo(todos); // Reescribimos el archivo completo con la lista modificada. 
    }

    // Reescribe el archivo completo con la nuevo archivo modificado.
    private void ReescribirArchivo(List<Expediente> expedientes)
    {
        using var sw = new StreamWriter(_nombreArchivo, false);
        foreach (var e in expedientes)
        {
            sw.WriteLine(e.Id);
            sw.WriteLine(e.Caratula!.Texto);
            sw.WriteLine(e.FechaCreacion.ToString("O"));
            sw.WriteLine(e.FechaUltimaModificacion.ToString("O"));
            sw.WriteLine(e.UsuarioUltimoCambio);
            sw.WriteLine(e.Estado);
        }
    }
}