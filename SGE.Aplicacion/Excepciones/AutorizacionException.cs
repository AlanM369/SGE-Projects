namespace SGE.Aplicacion.Excepciones;
// Heredamos de la clase base Exception de .NET
public class AutorizacionException : Exception
{   
    // Usamos el constructor base para pasarle el mensaje de error al sistema
    public AutorizacionException(string mensaje) : base(mensaje) { }
}