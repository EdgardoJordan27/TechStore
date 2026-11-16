using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _logger = logger;
        }

        private string UsuarioActual => User?.Identity?.Name ?? "anónimo";

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            _logger.LogInformation("Consulta de productos por {Usuario}: {Cantidad} registros", UsuarioActual, productos.Count);
            return View(productos);
        }

        [Authorize(Roles = RolesSistema.Personal)]
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            var modelo = new ProductoFormViewModel { Estado = EstadosProducto.Activo };
            await CargarCategoriasAsync(modelo);
            return View(modelo);
        }

        [Authorize(Roles = RolesSistema.Personal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(ProductoFormViewModel modelo)
        {
            await ValidarReferenciasAsync(modelo);

            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(modelo);
                return View(modelo);
            }

            try
            {
                await _productoService.AgregarAsync(ConvertirAEntidad(modelo));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al registrar el producto {Nombre}", modelo.Nombre);
                ModelState.AddModelError(string.Empty, "No se pudo registrar el producto. Intente nuevamente.");
                await CargarCategoriasAsync(modelo);
                return View(modelo);
            }

            _logger.LogInformation("Producto registrado: {Nombre} por {Usuario}", modelo.Nombre.Trim(), UsuarioActual);
            TempData["Mensaje"] = $"El producto \"{modelo.Nombre.Trim()}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RolesSistema.Personal)]
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto is null)
            {
                return NotFound();
            }

            var modelo = new ProductoFormViewModel
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                CategoriaId = producto.CategoriaId,
                Stock = producto.Stock,
                Estado = producto.Estado,
                Imagen = producto.Imagen
            };

            await CargarCategoriasAsync(modelo);
            return View(modelo);
        }

        [Authorize(Roles = RolesSistema.Personal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ProductoFormViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            await ValidarReferenciasAsync(modelo);

            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(modelo);
                return View(modelo);
            }

            bool actualizado;
            try
            {
                actualizado = await _productoService.EditarAsync(ConvertirAEntidad(modelo));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al editar el producto {Id}", modelo.Id);
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el producto. Intente nuevamente.");
                await CargarCategoriasAsync(modelo);
                return View(modelo);
            }

            if (!actualizado)
            {
                _logger.LogWarning("Se intentó editar un producto inexistente: {Id}", modelo.Id);
                return NotFound();
            }

            _logger.LogInformation("Producto editado: {Id} por {Usuario}", modelo.Id, UsuarioActual);

            TempData["Mensaje"] = $"El producto \"{modelo.Nombre.Trim()}\" se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RolesSistema.Administrador)]
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto is null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [Authorize(Roles = RolesSistema.Administrador)]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto is null)
            {
                return NotFound();
            }

            bool eliminado;
            try
            {
                eliminado = await _productoService.EliminarAsync(id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto {Id}", id);
                TempData["Error"] = "No se pudo eliminar el producto. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            if (!eliminado)
            {
                return NotFound();
            }

            _logger.LogWarning("Producto eliminado: {Id} ({Nombre}) por {Usuario}", id, producto.Nombre, UsuarioActual);

            TempData["Mensaje"] = $"El producto \"{producto.Nombre}\" se eliminó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCategoriasAsync(ProductoFormViewModel modelo)
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            modelo.Categorias = categorias
                .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
                .ToList();
        }

        private async Task ValidarReferenciasAsync(ProductoFormViewModel modelo)
        {
            if (modelo.CategoriaId.HasValue)
            {
                var categoria = await _categoriaService.ObtenerPorIdAsync(modelo.CategoriaId.Value);
                if (categoria is null)
                {
                    ModelState.AddModelError(nameof(modelo.CategoriaId), "La categoría seleccionada no existe.");
                }
            }

            if (!string.IsNullOrEmpty(modelo.Estado) && !EstadosProducto.Todos.Contains(modelo.Estado))
            {
                ModelState.AddModelError(nameof(modelo.Estado), "El estado seleccionado no es válido.");
            }
        }

        private static Producto ConvertirAEntidad(ProductoFormViewModel modelo)
        {
            return new Producto
            {
                Id = modelo.Id,
                Nombre = modelo.Nombre.Trim(),
                Descripcion = modelo.Descripcion.Trim(),
                Precio = modelo.Precio.GetValueOrDefault(),
                CategoriaId = modelo.CategoriaId.GetValueOrDefault(),
                Stock = modelo.Stock.GetValueOrDefault(),
                Estado = modelo.Estado,
                Imagen = string.IsNullOrWhiteSpace(modelo.Imagen) ? null : modelo.Imagen.Trim()
            };
        }
    }
}
