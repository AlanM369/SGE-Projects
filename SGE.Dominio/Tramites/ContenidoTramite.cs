using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class ContenidoTramite
{
    public string Texto { get; private set; }

    public ContenidoTramite(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DominioException("El contenido ingresado no es valido.");

        Texto = texto.Trim();
    }

    // Constructor protegido: requerido por EF Core
    protected ContenidoTramite()
    {
        Texto = null!;
    }

    public override string ToString() => Texto;
}