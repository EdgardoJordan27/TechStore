using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return View(productos);
        }

        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            var modelo = new ProductoFormViewModel { Estado = EstadosProducto.Activo };
            await CargarCategoriasAsync(modelo);
            return View(modelo);
        }

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

            await _productoService.AgregarAsync(ConvertirAEntidad(modelo));

            TempData["Mensaje"] = $"El producto \"{modelo.Nombre.Trim()}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

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

            var actualizado = await _productoService.EditarAsync(ConvertirAEntidad(modelo));
            if (!actualizado)
            {
                return NotFound();
            }

            TempData["Mensaje"] = $"El producto \"{modelo.Nombre.Trim()}\" se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

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

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto is null)
            {
                return NotFound();
            }

            var eliminado = await _productoService.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound();
            }

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
