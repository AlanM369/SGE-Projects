namespace SGE.Aplicacion.Comun;
// Heredamos de la clase base Exception de .NET
public class AutorizacionException : Exception
{   
    public AutorizacionException()
    {
    }

    // Usamos el constructor base para pasarle el mensaje de error al sistema
    public AutorizacionException(string? mensaje) : base(mensaje) { }

    public AutorizacionException(string? mensaje, Exception? innerException)
        : base(mensaje, innerException)
    {
    }
}