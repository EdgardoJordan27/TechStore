using Microsoft.AspNetCore.Mvc;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.ViewComponents
{
    // Resumen del inventario que se puede insertar en cualquier vista con
    // @await Component.InvokeAsync("ResumenInventario").
    public class ResumenInventarioViewComponent : ViewComponent
    {
        private const int LimiteStockBajo = 5;

        private readonly IProductoService _productoService;

        public ResumenInventarioViewComponent(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productos = await _productoService.ObtenerTodosAsync();

            var modelo = new ResumenInventarioViewModel
            {
                TotalProductos = productos.Count,
                Agotados = productos.Count(p => p.Stock == 0),
                StockBajo = productos.Count(p => p.Stock > 0 && p.Stock <= LimiteStockBajo),
                ValorInventario = productos.Sum(p => p.Precio * p.Stock)
            };

            return View(modelo);
        }
    }
}
