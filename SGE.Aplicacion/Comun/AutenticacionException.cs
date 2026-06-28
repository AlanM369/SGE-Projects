namespace SGE.Aplicacion.Comun;
public class AutenticacionException : Exception
{
    public AutenticacionException(string? mensaje) : base(mensaje) { }
}