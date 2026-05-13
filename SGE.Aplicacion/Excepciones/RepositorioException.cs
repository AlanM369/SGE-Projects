namespace SGE.Aplicacion.Excepciones;

public class RepositorioException : Exception
{
    public RepositorioException(string mensaje) : base(mensaje) { }
}