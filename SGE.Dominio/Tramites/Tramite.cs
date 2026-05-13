using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite
{
    // Las propiedades usan private set para garantizar el encapsulamiento.
    // Nadie desde afuera de esta clase puede modificar estos valores directamente.
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite? Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

    // Constructor natural: Se usa cuando el usuario crea un expediente desde cero.
    public Tramite(Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, Guid usuarioCreador)
    {
        Id = Guid.NewGuid(); // La entidad genera su propio ID.
        ExpedienteId = expedienteId;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = DateTime.Now;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioCreador;
    }

    // Constructor privado: Necesario para que el Factory Method pueda instanciar la clase sin pasar por las validaciones del alta.
    private Tramite() { }

    // Factory Method: Usado por la capa de Infraestructura para reconstruir el objeto cuando lea el archivo .txt.
    public static Tramite Reconstruir(Guid id, Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuario)
    {
        return new Tramite
        {
            Id = id,
            ExpedienteId = expedienteId,
            Etiqueta = etiqueta,
            Contenido = contenido,
            FechaCreacion = fechaCreacion,
            FechaUltimaModificacion = fechaModificacion,
            UsuarioUltimoCambio = usuario
        };
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