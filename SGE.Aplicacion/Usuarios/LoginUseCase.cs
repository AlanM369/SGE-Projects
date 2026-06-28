using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class LoginUseCase(IUsuarioRepository repositorio, IHashService hashService, ITokenProvider jwtService)
{
    public LoginResponse Ejecutar(LoginRequest request)
    {
        // 1. Buscamos el usuario por correo
        var usuario = repositorio.ObtenerPorCorreo(request.CorreoElectronico)
            ?? throw new AutenticacionException("Credenciales inválidas.");

        // 2. Verificamos la contraseña comparando hashes
        if (!hashService.Verificar(request.Contrasena, usuario.ContrasenaHash))
            throw new AutenticacionException("Credenciales inválidas.");

        // 3. Generamos el token JWT
        var token = jwtService.GenerarToken(usuario.Id);

        return new LoginResponse(token);
    }
}