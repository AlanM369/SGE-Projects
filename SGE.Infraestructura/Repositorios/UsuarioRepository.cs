using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;
using SGE.Infraestructura.Persistencia;

namespace SGE.Infraestructura.Repositorios;

// Repositorio de infraestructura para la persistencia y gestión de usuarios mediante Entity Framework Core
public class UsuarioRepository(SgeContext context) : IUsuarioRepository
{
    public void Agregar(Usuario usuario)
    {
        context.Usuarios.Add(usuario);
    }

    public Usuario? ObtenerPorId(Guid id)
    {
        return context.Usuarios
            .Where(u => u.Id == id)
            .SingleOrDefault();
    }

    public Usuario? ObtenerPorCorreo(string correo)
    {
        return context.Usuarios
            .Where(u => u.CorreoElectronico == correo.Trim().ToLower())
            .SingleOrDefault();
    }

    public IEnumerable<Usuario> ObtenerTodos()
    {
        return context.Usuarios.ToList();
    }

    public void Modificar(Usuario usuario)
    {
        // EF Core trackea los cambios automáticamente
    }

    public void Eliminar(Guid id)
    {
        var usuario = context.Usuarios
            .Where(u => u.Id == id)
            .SingleOrDefault();
        if (usuario != null)
            context.Usuarios.Remove(usuario);
    }
}
