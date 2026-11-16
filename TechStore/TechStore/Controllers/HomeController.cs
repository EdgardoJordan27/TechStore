using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;

        public HomeController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            var destacados = await _productoService.ObtenerDestacadosAsync(3);
            return View(destacados);
        }

        [HttpGet]
        public IActionResult Contacto()
        {
            return View(new ContactoViewModel());
        }

        [HttpPost]
        public IActionResult Contacto(ContactoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            ViewBag.Enviado = true;
            return View(new ContactoViewModel());
        }

        public IActionResult AcercaDe()
        {
            return View();
        }
    }
}
