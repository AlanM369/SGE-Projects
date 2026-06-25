namespace SGE.Aplicacion.Comun;

// Interfaz de abstracción del patrón Unit of Work
public interface IUnidadDeTrabajo
{
    // Confirma de forma atómica los cambios en la base de datos al finalizar el Caso de Uso
    void Guardar();
}