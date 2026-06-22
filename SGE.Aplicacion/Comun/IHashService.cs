namespace SGE.Aplicacion.Comun;

public interface IHashService
{
    string Hash(string texto);
    bool Verificar(string texto, string hash);
}