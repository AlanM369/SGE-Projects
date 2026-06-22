namespace SGE.Aplicacion.Comun;
// Heredamos de la clase base Exception de .NET

public class EntidadDuplicadaException : Exception
{
    public EntidadDuplicadaException() { }

    public EntidadDuplicadaException(string? mensaje) : base(mensaje) { }

    public EntidadDuplicadaException(string? mensaje, Exception? innerException)
        : base(mensaje, innerException) { }
}