using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using System.Collections.Generic;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        public static List<Producto> Lista = new List<Producto>
        {
            new Producto {
                Id = 1, Nombre = "Laptop HyperX Blaze Pro",
                Descripcion = "RTX 4070, 32GB RAM, 1TB SSD, pantalla 165Hz",
                Precio = 1299.00m, Categoria = "Laptops",
                Imagen = "productos/laptop.jpg", Stock = 12, Estado = "Activo"
            },
            new Producto {
                Id = 2, Nombre = "Smartphone Nova X1",
                Descripcion = "Pantalla AMOLED 120Hz, cámara 108MP, 256GB",
                Precio = 899.00m, Categoria = "Smartphones",
                Imagen = "productos/smartphone.jpg", Stock = 25, Estado = "Nuevo"
            },
            new Producto {
                Id = 3, Nombre = "Audifonos SonicPro NC",
                Descripcion = "Cancelación activa de ruido, 40h de batería",
                Precio = 249.00m, Categoria = "Audio",
                Imagen = "productos/audifonos.jpg", Stock = 8, Estado = "Activo"
            },
            new Producto {
                Id = 4, Nombre = "SmartWatch FitTrack 6",
                Descripcion = "Monitoreo de salud 24/7, GPS integrado, resistente al agua",
                Precio = 179.00m, Categoria = "Wearables",
                Imagen = "productos/smartwatch.jpg", Stock = 30, Estado = "Activo"
            },
            new Producto {
                Id = 5, Nombre = "Teclado Mecanico RGB Ducky",
                Descripcion = "Switches mecánicos, retroiluminación RGB personalizable",
                Precio = 149.00m, Categoria = "Perifericos",
                Imagen = "productos/teclado.jpg", Stock = 5, Estado = "Oferta"
            },
            new Producto {
                Id = 6, Nombre = "Monitor UltraWide 4K 34\"",
                Descripcion = "Panel IPS, 144Hz, ideal para trabajo y gaming",
                Precio = 599.00m, Categoria = "Monitores",
                Imagen = "productos/monitor.jpg", Stock = 0, Estado = "Agotado"
            },
            new Producto {
                Id = 7, Nombre = "Webcam 4K StreamPro",
                Descripcion = "Grabación 4K a 30fps, enfoque automático, micrófono dual",
                Precio = 129.00m, Categoria = "Perifericos",
                Imagen = "productos/webcam.jpg", Stock = 18, Estado = "Activo"
            },
            new Producto {
                Id = 8, Nombre = "Mouse Gaming Ultraligero",
                Descripcion = "63 gramos, sensor óptico 26000 DPI, cable paracord",
                Precio = 79.00m, Categoria = "Perifericos",
                Imagen = "productos/mouse.jpg", Stock = 42, Estado = "Activo"
            },
        };

        public IActionResult Index()
        {
            return View(Lista);
        }
    }
}