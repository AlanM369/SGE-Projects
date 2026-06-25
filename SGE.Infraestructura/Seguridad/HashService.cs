using System.Security.Cryptography;
using System.Text;
using SGE.Aplicacion.Comun;

namespace SGE.Infraestructura.Seguridad;

// Servicio encargado del cifrado y la validación de contraseñas
public class HashService : IHashService
{
    public string Hash(string texto)
    {
        // Conversión del texto plano a bytes y generación del hash criptográfico
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexString(bytes).ToLower();
    }

    public bool Verificar(string texto, string hash)
    {
        // Recálculo del hash del texto ingresado y comparación directa con el valor almacenado
        return Hash(texto) == hash;
    }
}