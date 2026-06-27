namespace SGE.Aplicacion.Comun;

// Interfaz para la generación de tokens de autenticación
public interface ITokenProvider
{
    string GenerarToken(Guid userId);
}