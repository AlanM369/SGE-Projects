namespace SGE.Aplicacion.Comun;
// Heredamos de la clase base Exception de .NET
public class EntidadNoEncontradaException : Exception
{   
    public EntidadNoEncontradaException()
    {
    }

    // Usamos el constructor base para pasarle el mensaje de error al sistema
    public EntidadNoEncontradaException(string? mensaje) : base(mensaje)
    {
    }
    public EntidadNoEncontradaException(string? mensaje, Exception? innerException)
        : base(mensaje, innerException)
    {
    }


}