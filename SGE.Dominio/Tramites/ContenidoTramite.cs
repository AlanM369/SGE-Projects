using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

// Usamos record class para representar datos inmutables y evitar utilizar primitivos.
public record class ContenidoTramite
{
    public string Texto { get; }// Readonly: Solo se puede asignar en el constructor, no tiene setter público.

    public ContenidoTramite(string texto)
    {
         // Se valida internamente al instanciar. Si falla, lanza la excepción de dominio.
        if (string.IsNullOrWhiteSpace(texto))
            throw new DominioException("El contenido ingresado no es valido.");

        Texto = texto.Trim();// Trim() es una gran práctica para borrar espacios al principio y final
    }
    // Sobrescribimos el comportamiento por defecto para que devuelva solo el texto
    public override string ToString() => Texto;

}