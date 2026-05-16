namespace SGE.Aplicacion.Excepciones;
// Heredamos de la clase base Exception de .NET
public class EntidadNoEncontradaException : Exception
{   
    // Usamos el constructor base para pasarle el mensaje de error al sistema
    public EntidadNoEncontradaException(string mensaje) : base(mensaje) { }
}