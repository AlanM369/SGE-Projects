using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Usuarios;

public record RegistrarUsuarioRequest(string Nombre, string CorreoElectronico, string Contrasena);
public record RegistrarUsuarioResponse(Guid Id);

public record LoginRequest(string CorreoElectronico, string Contrasena);
public record LoginResponse(string Token);

public record ModificarMisDatosRequest(Guid IdUsuario, string NuevoNombre, string NuevoCorreo, string? NuevaContrasena);
public record ModificarMisDatosResponse;

public record ModificarPermisosRequest(Guid IdAdministrador, Guid IdUsuarioObjetivo, List<Permiso> NuevosPermisos);
public record ModificarPermisosResponse;

public record EliminarUsuarioRequest(Guid IdAdministrador, Guid IdUsuarioAEliminar);
public record EliminarUsuarioResponse;

public record ListarUsuariosRequest(Guid IdAdministrador);

public record UsuarioDetalleDTO(Guid Id, string Nombre, string CorreoElectronico, bool EsAdministrador, IReadOnlyList<Permiso> Permisos);