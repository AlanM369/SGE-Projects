using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

// Usamos record class para representar datos inmutables y evitar utilizar primitivos.
public record class Caratula
{   
    public string Texto { get; }

    public Caratula(string texto)
    {   
        // Se valida internamente al instanciar. Si falla, lanza la excepción de dominio.
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new DominioException("El texto de la caratula es invalido.");
        }
        Texto = texto;
    }

}