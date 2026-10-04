using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using System.Linq;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var destacados = ProductosController.Lista
                .Where(p => p.Estado == "Activo")
                .OrderByDescending(p => p.Stock)
                .Take(3)
                .ToList();

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