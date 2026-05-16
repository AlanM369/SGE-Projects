using SGE.Dominio.Expedientes;
using System.Collections.Generic; //Necesitamos utilizar la interfaz IEnumerable<T>

namespace SGE.Aplicacion.Expedientes;

// La interfaz solo define las firmas de los métodos. 
// No le importa si guardamos en TXT, SQL o memoria.
public interface IExpedienteRepository
{
    void Agregar(Expediente expediente);
    Expediente? ObtenerPorId(Guid id); // El "?" significa que puede devolver null si no lo encuentra
    void Modificar(Expediente expediente);
    void Eliminar(Guid id);
    IEnumerable<Expediente> ObtenerTodos(); // IEnumerable es la interfaz base para listas
}