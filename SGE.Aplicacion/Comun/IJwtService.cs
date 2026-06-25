namespace SGE.Aplicacion.Comun;

// Interfaz para la generación de tokens de autenticación
public interface IJwtService
{
    string GenerarToken(Guid userId);
}