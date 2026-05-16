// SGE.Aplicacion/Comun/EntidadNoEncontradaException.cs

namespace SGE.Aplicacion.Comun;

public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string mensaje) : base(mensaje) { }
}