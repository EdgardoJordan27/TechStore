using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<List<Producto>> ObtenerDestacadosAsync(int cantidad);
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Producto producto);
        Task<bool> EditarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
    }
}
