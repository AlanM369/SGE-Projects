using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite
{
    // Las propiedades usan private set para garantizar el encapsulamiento.
    // Nadie desde afuera de esta clase puede modificar estos valores directamente.
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; } // El ID del expediente al que pertenece el trámite.
    public EtiquetaTramite Etiqueta { get; private set; } // Enumerativo
    public ContenidoTramite? Contenido { get; private set; } // ValueObject, permite que si existe, no sea nulo.
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

    // Constructor privado: Centraliza toda la lógica de construcción.
    // Es utilizado tanto por el constructor público como por el Factory Method.
    private Tramite(
        Guid id,
        Guid expedienteId,
        EtiquetaTramite etiqueta,
        ContenidoTramite contenido,
        DateTime fechaCreacion,
        DateTime fechaUltimaModificacion,
        Guid usuarioUltimoCambio)
    {
        Id = id;
        ExpedienteId = expedienteId;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }


    // Constructor público: Se usa cuando el usuario crea un trámite nuevo.
    // Genera automáticamente el ID y las fechas iniciales.
    public Tramite(
        Guid expedienteId,
        EtiquetaTramite etiqueta,
        ContenidoTramite contenido,
        Guid usuarioCreador)
        : this(
            Guid.NewGuid(), // Genera un ID nuevo para el trámite.
            expedienteId, // El ID del expediente al que pertenece el trámite.
            etiqueta,
            contenido,
            DateTime.Now,
            DateTime.Now,
            usuarioCreador)
    {
    }


    // Factory Method: Usado por la capa de Infraestructura para reconstruir
    // el objeto cuando se lee desde el archivo .txt.
    // No genera nuevos datos, sino que reutiliza los persistidos.
    public static Tramite Reconstruir(
        Guid id,
        Guid expedienteId,
        EtiquetaTramite etiqueta,
        ContenidoTramite contenido,
        DateTime fechaCreacion,
        DateTime fechaModificacion,
        Guid usuario)
    {
        return new Tramite(
            id,
            expedienteId,
            etiqueta,
            contenido,
            fechaCreacion,
            fechaModificacion,
            usuario);
    }

    // Método para alterar el trámite asegurando que se respeten las reglas de negocio.
    public void ModificarTramite(EtiquetaTramite nuevaEtiqueta, ContenidoTramite nuevoContenido, Guid idUsuario)
    {
        Etiqueta = nuevaEtiqueta;
        Contenido = nuevoContenido;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;

        // Centraliza la validación de la invariante de fechas.
        if (FechaUltimaModificacion < FechaCreacion)
        {
            throw new DominioException("La fecha de modificacion no puede ser menor a la de creacion.");
        }
    }
}