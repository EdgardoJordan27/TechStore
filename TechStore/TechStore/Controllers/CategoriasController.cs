using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        private string UsuarioActual => User?.Identity?.Name ?? "anónimo";

        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            _logger.LogInformation("Consulta de categorías por {Usuario}: {Cantidad} registros", UsuarioActual, categorias.Count);
            return View(categorias);
        }

        [Authorize(Roles = RolesSistema.Administrador)]
        [HttpGet]
        public IActionResult Agregar()
        {
            return View(new CategoriaFormViewModel { Icono = IconosCategoria.Predeterminado });
        }

        [Authorize(Roles = RolesSistema.Administrador)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(CategoriaFormViewModel modelo)
        {
            ValidarIcono(modelo);

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                await _categoriaService.AgregarAsync(ConvertirAEntidad(modelo));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al registrar la categoría {Nombre}", modelo.Nombre);
                ModelState.AddModelError(string.Empty, "No se pudo registrar la categoría. Intente nuevamente.");
                return View(modelo);
            }

            _logger.LogInformation("Categoría registrada: {Nombre} por {Usuario}", modelo.Nombre.Trim(), UsuarioActual);
            TempData["Mensaje"] = $"La categoría \"{modelo.Nombre.Trim()}\" se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RolesSistema.Administrador)]
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

        [Authorize(Roles = RolesSistema.Administrador)]
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

            bool actualizada;
            try
            {
                actualizada = await _categoriaService.EditarAsync(ConvertirAEntidad(modelo));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al editar la categoría {Id}", modelo.Id);
                ModelState.AddModelError(string.Empty, "No se pudo actualizar la categoría. Intente nuevamente.");
                return View(modelo);
            }

            if (!actualizada)
            {
                _logger.LogWarning("Se intentó editar una categoría inexistente: {Id}", modelo.Id);
                return NotFound();
            }

            _logger.LogInformation("Categoría editada: {Id} por {Usuario}", modelo.Id, UsuarioActual);

            TempData["Mensaje"] = $"La categoría \"{modelo.Nombre.Trim()}\" se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RolesSistema.Administrador)]
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

        [Authorize(Roles = RolesSistema.Administrador)]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            ResultadoEliminacion resultado;
            try
            {
                resultado = await _categoriaService.EliminarAsync(id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría {Id}", id);
                TempData["Error"] = "No se pudo eliminar la categoría. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Eliminación de categoría {Id} por {Usuario}: {Resultado}", id, UsuarioActual, resultado);

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
