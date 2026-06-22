using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Repositorios;


//Queda mas parecido a los otros dos repositorios, temrminar de chequear
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


// using SGE.Aplicacion.Usuarios;
// using SGE.Dominio.Autorizacion;
// using SGE.Dominio.Usuarios;

// namespace SGE.Infraestructura.Repositorios;

// public class UsuarioRepository(SgeContext context) : IUsuarioRepository
// {
//     public void Agregar(Usuario usuario)
//     {
//         context.Usuarios.Add(usuario);
//         GuardarPermisos(usuario);
//     }

//     public Usuario? ObtenerPorId(Guid id)
//     {
//         var usuario = context.Usuarios
//             .Where(u => u.Id == id)
//             .SingleOrDefault();
//         if (usuario != null)
//             CargarPermisos(usuario);
//         return usuario;
//     }

//     public Usuario? ObtenerPorCorreo(string correo)
//     {
//         var usuario = context.Usuarios
//             .Where(u => u.CorreoElectronico == correo.Trim().ToLower())
//             .SingleOrDefault();
//         if (usuario != null)
//             CargarPermisos(usuario);
//         return usuario;
//     }

//     public IEnumerable<Usuario> ObtenerTodos()
//     {
//         var usuarios = context.Usuarios.ToList();
//         foreach (var u in usuarios)
//             CargarPermisos(u);
//         return usuarios;
//     }

//     public void Modificar(Usuario usuario)
//     {
//         // EF Core trackea los cambios automáticamente,
//         // no es necesario llamar a Update()
//         GuardarPermisos(usuario);
//     }

//     public void Eliminar(Guid id)
//     {
//         var usuario = context.Usuarios
//             .Where(u => u.Id == id)
//             .SingleOrDefault();
//         if (usuario != null)
//             context.Usuarios.Remove(usuario);
//     }

//     // --- Helpers para serializar/deserializar permisos ---

//     private void GuardarPermisos(Usuario usuario)
//     {
//         var serializado = string.Join(",", usuario.Permisos.Select(p => p.ToString()));
//         context.Entry(usuario).Property<string>("PermisosSerializados").CurrentValue = serializado;
//     }

//     private void CargarPermisos(Usuario usuario)
//     {
//         var serializado = context.Entry(usuario).Property<string>("PermisosSerializados").CurrentValue;
//         if (string.IsNullOrWhiteSpace(serializado))
//             return;

//         var permisos = serializado
//             .Split(',', StringSplitOptions.RemoveEmptyEntries)
//             .Select(p => Enum.Parse<Permiso>(p));

//         foreach (var permiso in permisos)
//             usuario.AgregarPermiso(permiso);
//     }
// }