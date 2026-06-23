using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

public class Caratula
{
    public string Texto { get; private set; }

    public Caratula(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DominioException("El texto de la caratula es invalido.");

        Texto = texto.Trim();
    }

    // Constructor protegido: requerido por EF Core
    protected Caratula()
    {
        Texto = null!;
    }

    public override string ToString() => Texto;
}