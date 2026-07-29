using BackendApi.Models;

namespace BackendApi.Data;

public interface IRepository<T> where T : class, IEntidad
{
    Task<IReadOnlyList<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(int id);
    Task<T> CrearAsync(T entidad);
    Task<T?> ActualizarAsync(int id, T entidad);
    Task<bool> EliminarAsync(int id);
}
