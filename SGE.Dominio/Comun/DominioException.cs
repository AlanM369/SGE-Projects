namespace SGE.Dominio.Comun;

public class DominioException : Exception
{
    public DominioException()
    {     
    }
    
    public DominioException(string? mensaje) : base(mensaje)
    {    
    }

    public DominioException(string? mensaje, Exception? innerException)
        : base(mensaje, innerException)
    {    
    }


}
