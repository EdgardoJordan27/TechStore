using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductoService productoService, ILogger<HomeController> logger)
        {
            _productoService = productoService;
            _logger = logger;
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

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? codigo)
        {
            var modelo = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Codigo = codigo ?? 500
            };

            _logger.LogWarning("Se mostró la página de error. Código {Codigo}, solicitud {RequestId}",
                modelo.Codigo, modelo.RequestId);

            return View(modelo);
        }

        // Solo para demostrar el manejo global de excepciones y su registro en el log.
        [Authorize(Roles = RolesSistema.Administrador)]
        public IActionResult SimularError()
        {
            _logger.LogInformation("Se solicitó una excepción de prueba.");
            throw new InvalidOperationException("Excepción de prueba generada desde Home/SimularError.");
        }
    }
}
