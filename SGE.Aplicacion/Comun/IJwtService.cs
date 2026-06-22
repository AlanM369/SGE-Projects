namespace SGE.Aplicacion.Comun;

//Falta definir la clase que genera el token
public interface IJwtService
{
    string GenerarToken(Guid userId);
}