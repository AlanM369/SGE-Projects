namespace SGE.Infraestructura;

// Lanzada cuando el repositorio no encuentra una entidad para modificar o eliminar.
public class RepositorioException : Exception
{
    public RepositorioException(string mensaje) : base(mensaje) { }
}