using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

// Usamos record class para representar datos inmutables y evitar utilizar primitivos.
public record class ContenidoTramite
{
    public string Texto { get; }

    public ContenidoTramite(string texto)
    {
         // Se valida internamente al instanciar. Si falla, lanza la excepción de dominio.
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new DominioException("El contenido ingresado no es valido.");
        }
        Texto = texto;
    }
}