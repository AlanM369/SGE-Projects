namespace SGE.Aplicacion.Comun;

// Interfaz para el manejo y abstracción del hash de contraseñas
public interface IHashService
{
    string Hash(string texto);
    bool Verificar(string texto, string hash);
}