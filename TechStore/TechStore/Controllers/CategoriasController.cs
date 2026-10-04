using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using System.Collections.Generic;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        public static List<Categoria> Lista = new List<Categoria>
        {
            new Categoria {
                Id = 1, Nombre = "Laptops & Computadoras",
                Descripcion = "Portátiles gaming, ultrabooks y estaciones de trabajo de alto rendimiento.",
                Icono = "bi-laptop", CantidadProductos = 1, PrecioDesde = 1299.00m
            },
            new Categoria {
                Id = 2, Nombre = "Smartphones",
                Descripcion = "Teléfonos flagship, gama media y accesorios móviles.",
                Icono = "bi-phone", CantidadProductos = 1, PrecioDesde = 899.00m
            },
            new Categoria {
                Id = 3, Nombre = "Audio & Sonido",
                Descripcion = "Audífonos con cancelación de ruido, parlantes y barras de sonido.",
                Icono = "bi-headphones", CantidadProductos = 1, PrecioDesde = 249.00m
            },
            new Categoria {
                Id = 4, Nombre = "Wearables",
                Descripcion = "Smartwatches, fitness trackers y dispositivos de salud inteligente.",
                Icono = "bi-smartwatch", CantidadProductos = 1, PrecioDesde = 179.00m
            },
            new Categoria {
                Id = 5, Nombre = "Perifericos & Gaming",
                Descripcion = "Teclados mecánicos, mice, webcams y accesorios RGB.",
                Icono = "bi-keyboard", CantidadProductos = 4, PrecioDesde = 79.00m
            },
        };

        public IActionResult Index()
        {
            return View(Lista);
        }
    }
}
