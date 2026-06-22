using SGE.Dominio.Autorizacion;
using SGE.Dominio.Comun;

//Chequear metodos

namespace SGE.Dominio.Usuarios;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public string CorreoElectronico { get; private set; }
    public string ContrasenaHash { get; private set; }
    public bool EsAdministrador { get; private set; }

    
    private readonly List<Permiso> _permisos = [];
    public IReadOnlyList<Permiso> Permisos => _permisos.AsReadOnly();

    // Constructor público: crea un usuario nuevo desde cero
    public Usuario(string nombre, string correoElectronico, string contrasenaHash)
        : this(Guid.NewGuid(), nombre, correoElectronico, contrasenaHash, false)
    {
    }

    // Constructor privado: centraliza validaciones
    private Usuario(Guid id, string nombre, string correoElectronico, string contrasenaHash, bool esAdministrador)
    {
        if (id == Guid.Empty)
            throw new DominioException("El Id del usuario no puede ser un Guid vacío.");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(correoElectronico))
            throw new DominioException("El correo electrónico del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(contrasenaHash))
            throw new DominioException("La contraseña del usuario es obligatoria.");

        Id = id;
        Nombre = nombre.Trim();
        CorreoElectronico = correoElectronico.Trim().ToLower();
        ContrasenaHash = contrasenaHash;
        EsAdministrador = esAdministrador;
    }

    // Constructor protegido: requerido por EF Core para reconstruir entidades desde la BD
    protected Usuario()
    {
        Nombre = string.Empty;
        CorreoElectronico = string.Empty;
        ContrasenaHash = string.Empty;
    }

    // Factory Method: para reconstruir desde la BD o crear el administrador semilla
    public static Usuario Reconstruir(Guid id, string nombre, string correo, string contrasenaHash, bool esAdministrador)
    {
        return new(id, nombre, correo, contrasenaHash, esAdministrador);
    }

    // --- Métodos de negocio ---


    public void ModificarDatos(string nuevoNombre, string nuevoCorreo)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new DominioException("El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(nuevoCorreo))
            throw new DominioException("El correo electrónico del usuario es obligatorio.");

        Nombre = nuevoNombre.Trim();
        CorreoElectronico = nuevoCorreo.Trim().ToLower();
    }

    public void CambiarContrasena(string nuevoHash)
    {
        if (string.IsNullOrWhiteSpace(nuevoHash))
            throw new DominioException("La contraseña no puede estar vacía.");

        ContrasenaHash = nuevoHash;
    }

    public void AgregarPermiso(Permiso permiso)
    {
        if (!_permisos.Contains(permiso))
            _permisos.Add(permiso);
    }

    public void RemoverPermiso(Permiso permiso)
    {
        _permisos.Remove(permiso);
    }

    public void ReemplazarPermisos(IEnumerable<Permiso> nuevosPermisos)
    {
        _permisos.Clear();
        _permisos.AddRange(nuevosPermisos.Distinct());
    }
}