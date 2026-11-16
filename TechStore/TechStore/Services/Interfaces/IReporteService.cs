using TechStore.Models;

namespace TechStore.Services.Interfaces
{
    public interface IReporteService
    {
        byte[] GenerarPdfProductos(IEnumerable<Producto> productos);
        byte[] GenerarExcelProductos(IEnumerable<Producto> productos);
    }
}
