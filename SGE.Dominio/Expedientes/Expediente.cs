using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Dominio.Expedientes;

public class Expediente
{
    // Las propiedades usan private set para garantizar el encapsulamiento.
    // Nadie desde afuera de esta clase puede modificar estos valores directamente.
    public Guid Id {get; private set;}
    public Caratula? Caratula { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoExpediente Estado { get; private set; }

    // Constructor natural: Se usa cuando el usuario crea un expediente desde cero.
    public Expediente(Caratula caratula, Guid usuarioCreador)
    {
        Id = Guid.NewGuid(); // La entidad genera su propio ID.
        Caratula = caratula;
        FechaCreacion = DateTime.Now;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioCreador;
        Estado = EstadoExpediente.RecienIniciado;
    }

    // Constructor privado: Necesario para que el Factory Method pueda instanciar la clase sin pasar por las validaciones del alta.
    private Expediente() { }

    // Factory Method: Usado por la capa de Infraestructura para reconstruir el objeto cuando lea el archivo .txt.
    public static Expediente Reconstruir(Guid id, Caratula caratula, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuario, EstadoExpediente estado)
    {
        return new Expediente
        {
            Id = id,
            Caratula = caratula,
            FechaCreacion = fechaCreacion,
            FechaUltimaModificacion = fechaModificacion,
            UsuarioUltimoCambio = usuario,
            Estado = estado
        };
    }

    // A. Modificar Carátula: Operación excepcional que no afecta el flujo de vida natural.
    public void ModificarCaratula(Caratula nuevaCaratula, Guid idUsuario)
    {
        Caratula = nuevaCaratula;
        ActualizarAuditoria(idUsuario);
    }

    // B. Cambio de Estado Automático: La entidad evalúa su propio estado según la etiqueta del último trámite.
    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        EstadoExpediente nuevoEstado;

        // Mapeo de reglas de negocio según la etiqueta.
        if (ultimaEtiqueta == null)
            nuevoEstado = EstadoExpediente.RecienIniciado;
        else if (ultimaEtiqueta == EtiquetaTramite.Resolucion)
            nuevoEstado = EstadoExpediente.ConResolucion;
        else if (ultimaEtiqueta == EtiquetaTramite.PaseAEstudio)
            nuevoEstado = EstadoExpediente.ParaResolver;
        else if (ultimaEtiqueta == EtiquetaTramite.PaseAlArchivo)
            nuevoEstado = EstadoExpediente.Finalizado;
        else
            return false;

        // Solo se actualiza si el estado realmente cambió, retornando true para avisar a la capa de Aplicación.
        if (Estado != nuevoEstado)
        {
            Estado = nuevoEstado;
            ActualizarAuditoria(idUsuario);
            return true;
        }

        return false;
    }

    // C. Cambio de Estado Manual: Permite forzar un estado independientemente de los trámites.
    public void CambiarEstado(EstadoExpediente nuevoEstado, Guid idUsuario)
    {
        Estado = nuevoEstado;
        ActualizarAuditoria(idUsuario);
    }

    // Método auxiliar para no repetir código. Centraliza la validación de la invariante de fechas.
    private void ActualizarAuditoria(Guid idUsuario)
    {
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
        
        if (FechaUltimaModificacion < FechaCreacion)
        {
            throw new DominioException("La fecha de modificacion no puede ser menor a la de creacion.");
        }
    }
}