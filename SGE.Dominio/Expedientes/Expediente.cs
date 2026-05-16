using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Dominio.Expedientes;

public class Expediente
{   
    // Las propiedades usan private set para garantizar el encapsulamiento.
    // Nadie desde afuera de esta clase puede modificar estos valores directamente.
    public Guid Id {get; private set;}
    public Caratula Caratula { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoExpediente Estado { get; private set; }

    // Constructor publico: Se usa cuando el usuario crea un expediente desde cero. Genera un ID nuevo y llama a la constructor privado.
    public Expediente(Caratula caratula, Guid usuario) : this(Guid.NewGuid(), caratula, DateTime.Now, DateTime.Now, usuario, EstadoExpediente.RecienIniciado)
    {
    }

    // Constructor privado: Necesario para que el Factory Method pueda instanciar la clase sin pasar por las validaciones del alta.
     private Expediente( Guid id, Caratula caratula, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio, EstadoExpediente estado)
    {   
        if(id == Guid.Empty)
            throw new DominioException("El id del expediente no puede ser un Guid vacio");
        
        if(fechaCreacion == default)
            throw new DominioException("La fecha de creacion del expediente es obligatoria.");
        
        if(fechaUltimaModificacion == default)
            throw new DominioException("La fecha de modificacion del expediente es obligatoria.");
        
        if(usuarioUltimoCambio == Guid.Empty)
            throw new DominioException("El id del usuario responsable del ultimo cambio es obligatorio.");

        // Centraliza la validación de la invariante de fechas.
        if (fechaUltimaModificacion < fechaCreacion)
            throw new DominioException("La fecha de modificacion no puede ser menor a la de creacion.");

        Id = id;
        Caratula = caratula ?? throw new DominioException("La caratula del expediente es obligatoria.");;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = estado;
    }

    // Factory Method: Usado por la capa de Infraestructura para reconstruir el objeto cuando lea el archivo .txt.
    public static Expediente Reconstruir(Guid id, Caratula caratula, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuario, EstadoExpediente estado)
    {
        return new Expediente(id, caratula, fechaCreacion, fechaModificacion, usuario, estado);
    }

    // A. Modificar Carátula: Si el usuario cometió un error al ingresar el expediente, debe poder corregir la 
    // carátula. Esta es una operación excepcional que no afecta el flujo de vida del expediente.
    public void ModificarCaratula(Caratula nuevaCaratula, Guid idUsuario)
    {
        Caratula = nuevaCaratula;
        ActualizarAuditoria(idUsuario);
    }

    // B. Cambio de Estado Automático (Por trámites): Al agregar, modificar o eliminar un trámite, el sistema
    // puede cambiar automáticamente el estado del expediente según la etiqueta del trámite que haya quedado
    // como "último" (considerando como "último" a aquel trámite con la fecha de creación más reciente):
    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        EstadoExpediente nuevoEstado;

        // Mapeo de reglas de negocio según la etiqueta.
        switch (ultimaEtiqueta)
        {
            case null: nuevoEstado = EstadoExpediente.RecienIniciado;
            break;
            case EtiquetaTramite.Resolucion: nuevoEstado = EstadoExpediente.ConResolucion;
            break;
            case EtiquetaTramite.PaseAEstudio: nuevoEstado = EstadoExpediente.ParaResolver;
            break;
            case EtiquetaTramite.PaseAlArchivo: nuevoEstado = EstadoExpediente.Finalizado;
            break;
            default: return false;
        }
        // Solo se actualiza si el estado realmente cambió, retornando true para avisar a la capa de Aplicación.
        if (Estado != nuevoEstado)
        {
            Estado = nuevoEstado;
            ActualizarAuditoria(idUsuario);
            return true;
        }

        return false;
    }

     // C. Cambio de Estado Manual (Flujo natural): El usuario debe poder cambiar el estado de un expediente
    // manualmente, sin necesidad de realizar un trámite.
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