using SGE.Dominio.Comun;

namespace SGE.Aplicacion.Comun;

// Heredamos de la clase base Exception de .NET
public class RepositorioException : Exception
{   
    public RepositorioException()
    {
    }

    // Usamos el constructor base para pasarle el mensaje de error al sistema
    public RepositorioException(string? mensaje) : base(mensaje)
    {
    }

    public RepositorioException(string? mensaje, Exception? innerException)
        : base(mensaje, innerException)
    {
    }

}