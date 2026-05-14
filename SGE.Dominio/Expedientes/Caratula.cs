using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

// Usamos record class para representar datos inmutables y evitar utilizar primitivos.
public record class Caratula
{   
    public string Texto { get; } // Readonly: Solo se puede asignar en el constructor, no tiene setter público.

    public Caratula(string texto)
    {   
        // Se valida internamente al instanciar. Si falla, lanza la excepción de dominio.
        if (string.IsNullOrWhiteSpace(texto)) // Validación básica: No puede ser nulo, vacío o solo espacios.
        {
            throw new DominioException("El texto de la caratula es invalido."); // Si la validación falla, se lanza una excepción de dominio.
        }
        Texto = texto; // Si la validación pasa, se asigna el valor que es de solo lectura (get).
    }

}