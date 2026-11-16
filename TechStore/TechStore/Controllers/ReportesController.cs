using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Controllers
{
    [Authorize(Roles = RolesSistema.Personal)]
    public class ReportesController : Controller
    {
        private const string TipoPdf = "application/pdf";
        private const string TipoExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IProductoService _productoService;
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            IProductoService productoService,
            IReporteService reporteService,
            ILogger<ReportesController> logger)
        {
            _productoService = productoService;
            _reporteService = reporteService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Pdf()
        {
            try
            {
                var productos = await _productoService.ObtenerTodosAsync();
                var contenido = _reporteService.GenerarPdfProductos(productos);

                _logger.LogInformation("Reporte PDF generado por {Usuario}: {Cantidad} productos",
                    User?.Identity?.Name, productos.Count);

                return File(contenido, TipoPdf, $"Reporte_Productos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte PDF");
                TempData["Error"] = "No se pudo generar el reporte PDF. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Excel()
        {
            try
            {
                var productos = await _productoService.ObtenerTodosAsync();
                var contenido = _reporteService.GenerarExcelProductos(productos);

                _logger.LogInformation("Reporte Excel generado por {Usuario}: {Cantidad} productos",
                    User?.Identity?.Name, productos.Count);

                return File(contenido, TipoExcel, $"Productos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el archivo Excel");
                TempData["Error"] = "No se pudo generar el archivo Excel. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
