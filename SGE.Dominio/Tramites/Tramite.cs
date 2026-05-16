using SGE.Dominio.Comun;
namespace SGE.Dominio.Tramites;

public class Tramite
{
    // Las propiedades usan private set para garantizar el encapsulamiento.
    // Nadie desde afuera de esta clase puede modificar estos valores directamente.
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

     // Constructor público: Se usa cuando el usuario crea un trámite nuevo.
    // Genera automáticamente el ID y las fechas iniciales.
    public Tramite(Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, Guid usuarioCreador) : this(Guid.NewGuid(), expedienteId, etiqueta, contenido, DateTime.Now, DateTime.Now, usuarioCreador)
    {
    }

    // Constructor privado: Centraliza toda la lógica de construcción.
    // Es utilizado tanto por el constructor público como por el Factory Method.
     private Tramite(Guid id, Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio)
    {   
        if(id == Guid.Empty)
            throw new DominioException("El id del tramite no puede ser un Guid vacio");

        if(expedienteId == Guid.Empty)
            throw new DominioException("El Id del expediente es obligatorio.");
        
        if(fechaCreacion == default)
            throw new DominioException("La fecha de creacion del tramite es obligatoria.");
        
        if(fechaUltimaModificacion == default)
            throw new DominioException("La fecha de modificacion del tramite es obligatoria.");
        
        if(usuarioUltimoCambio == Guid.Empty)
            throw new DominioException("El id del usuario responsable del ultimo cambio es obligatorio.");

        // Centraliza la validación de la invariante de fechas.
        ValidarFechas(FechaCreacion, FechaUltimaModificacion);

        Id = id;
        ExpedienteId = expedienteId;
        Etiqueta = etiqueta; // Los enumerativos no se validan por null
        Contenido = contenido ?? throw new DominioException("El contenido del tramite es obligatorio.");
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }

    // Factory Method: Usado por la capa de Infraestructura para reconstruir el objeto cuando lea el archivo .txt.
    public static Tramite Reconstruir(Guid id, Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuario)
    {
        return new Tramite(id, expedienteId, etiqueta, contenido, fechaCreacion, fechaModificacion, usuario);
    }

    // Método para alterar el trámite asegurando que se respeten las reglas de negocio.
    public void ModificarTramite(EtiquetaTramite nuevaEtiqueta, ContenidoTramite nuevoContenido, Guid idUsuario)
    {
        Etiqueta = nuevaEtiqueta;
        Contenido = nuevoContenido;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;

        // Centraliza la validación de la invariante de fechas.
        ValidarFechas(FechaCreacion, FechaUltimaModificacion);
    }
    private void ValidarFechas (DateTime fechaCreacion, DateTime fechaModificacion)
    {
        if (fechaModificacion < fechaCreacion)
            throw new DominioException("La fecha de última modificacion no puede ser anterior a la fecha de creación");
    }
}