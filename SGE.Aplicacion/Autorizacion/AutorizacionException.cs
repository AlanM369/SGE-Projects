namespace SGE.Aplicacion.Autorizacion;

public class AutorizacionException : Exception
{
    public AutorizacionException(string mensaje) : base(mensaje) { }
}