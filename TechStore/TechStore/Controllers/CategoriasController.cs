using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            return View(new CategoriaFormViewModel { Icono = IconosCategoria.Predeterminado });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(CategoriaFormViewModel modelo)
        {
            ValidarIcono(modelo);

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            await _categoriaService.AgregarAsync(ConvertirAEntidad(modelo));

            TempData["Mensaje"] = $"La categoría \"{modelo.Nombre.Trim()}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria is null)
            {
                return NotFound();
            }

            var modelo = new CategoriaFormViewModel
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Icono = categoria.Icono
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CategoriaFormViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            ValidarIcono(modelo);

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var actualizada = await _categoriaService.EditarAsync(ConvertirAEntidad(modelo));
            if (!actualizada)
            {
                return NotFound();
            }

            TempData["Mensaje"] = $"La categoría \"{modelo.Nombre.Trim()}\" se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria is null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var resultado = await _categoriaService.EliminarAsync(id);

            switch (resultado)
            {
                case ResultadoEliminacion.NoEncontrada:
                    return NotFound();

                case ResultadoEliminacion.TieneProductos:
                    TempData["Error"] = "No se puede eliminar la categoría porque todavía tiene productos asociados.";
                    return RedirectToAction(nameof(Index));

                default:
                    TempData["Mensaje"] = "La categoría se eliminó correctamente.";
                    return RedirectToAction(nameof(Index));
            }
        }

        private void ValidarIcono(CategoriaFormViewModel modelo)
        {
            var existe = IconosCategoria.Disponibles.Any(i => i.Clase == modelo.Icono);
            if (!string.IsNullOrEmpty(modelo.Icono) && !existe)
            {
                ModelState.AddModelError(nameof(modelo.Icono), "El ícono seleccionado no es válido.");
            }
        }

        private static Categoria ConvertirAEntidad(CategoriaFormViewModel modelo)
        {
            return new Categoria
            {
                Id = modelo.Id,
                Nombre = modelo.Nombre.Trim(),
                Descripcion = modelo.Descripcion.Trim(),
                Icono = modelo.Icono
            };
        }
    }
}
