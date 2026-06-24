using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

// Usamos record class para representar datos inmutables y evitar utilizar primitivos.
public record class Caratula
{   
    public string Texto { get; } // Readonly: Solo se puede asignar en el constructor, no tiene setter público.

    public Caratula(string texto)
    {   
        // Validacion basica internamente al instanciar, no puede ser nulo, vacío o solo espacios.
        if (string.IsNullOrWhiteSpace(texto))
            throw new DominioException("El texto de la caratula es invalido."); // Si la validación falla, se lanza una excepción de dominio.
        
        Texto = texto.Trim(); // Trim() es una gran práctica para borrar espacios al principio y final
    }

    // Sobrescribimos el comportamiento por defecto para que devuelva solo el texto
    public override string ToString() => Texto;

}