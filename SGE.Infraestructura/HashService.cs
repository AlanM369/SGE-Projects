using System.Security.Cryptography;
using System.Text;
using SGE.Aplicacion.Comun;

//Hash de la contraseña
namespace SGE.Infraestructura;

public class HashService : IHashService
{
    public string Hash(string texto)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexString(bytes).ToLower();
    }

    public bool Verificar(string texto, string hash)
    {
        return Hash(texto) == hash;
    }
}