using TechStore.Models;

namespace TechStore.Data
{
    // Datos de la Unidad I que antes vivían en listas estáticas dentro de los controladores.
    public static class DatosIniciales
    {
        // Se siembran como tipos anónimos porque Categoria tiene una colección de navegación
        // (Productos) y HasData solo admite valores escalares y claves foráneas.
        public static object[] ObtenerCategorias()
        {
            return new object[]
            {
                new
                {
                    Id = 1, Nombre = "Laptops & Computadoras",
                    Descripcion = "Portátiles gaming, ultrabooks y estaciones de trabajo de alto rendimiento.",
                    Icono = "bi-laptop"
                },
                new
                {
                    Id = 2, Nombre = "Smartphones",
                    Descripcion = "Teléfonos flagship, gama media y accesorios móviles.",
                    Icono = "bi-phone"
                },
                new
                {
                    Id = 3, Nombre = "Audio & Sonido",
                    Descripcion = "Audífonos con cancelación de ruido, parlantes y barras de sonido.",
                    Icono = "bi-headphones"
                },
                new
                {
                    Id = 4, Nombre = "Wearables",
                    Descripcion = "Smartwatches, fitness trackers y dispositivos de salud inteligente.",
                    Icono = "bi-smartwatch"
                },
                new
                {
                    Id = 5, Nombre = "Periféricos & Gaming",
                    Descripcion = "Teclados mecánicos, mice, webcams y accesorios RGB.",
                    Icono = "bi-keyboard"
                },
                new
                {
                    Id = 6, Nombre = "Monitores & Pantallas",
                    Descripcion = "Monitores ultrawide, 4K y de alta tasa de refresco.",
                    Icono = "bi-display"
                }
            };
        }

        public static Producto[] ObtenerProductos()
        {
            return new[]
            {
                new Producto
                {
                    Id = 1, Nombre = "Laptop HyperX Blaze Pro",
                    Descripcion = "RTX 4070, 32GB RAM, 1TB SSD, pantalla 165Hz",
                    Precio = 1299.00m, CategoriaId = 1,
                    Imagen = "productos/laptop.jpg", Stock = 12, Estado = EstadosProducto.Activo
                },
                new Producto
                {
                    Id = 2, Nombre = "Smartphone Nova X1",
                    Descripcion = "Pantalla AMOLED 120Hz, cámara 108MP, 256GB",
                    Precio = 899.00m, CategoriaId = 2,
                    Imagen = "productos/smartphone.jpg", Stock = 25, Estado = EstadosProducto.Nuevo
                },
                new Producto
                {
                    Id = 3, Nombre = "Audífonos SonicPro NC",
                    Descripcion = "Cancelación activa de ruido, 40h de batería",
                    Precio = 249.00m, CategoriaId = 3,
                    Imagen = "productos/audifonos.jpg", Stock = 8, Estado = EstadosProducto.Activo
                },
                new Producto
                {
                    Id = 4, Nombre = "SmartWatch FitTrack 6",
                    Descripcion = "Monitoreo de salud 24/7, GPS integrado, resistente al agua",
                    Precio = 179.00m, CategoriaId = 4,
                    Imagen = "productos/smartwatch.jpg", Stock = 30, Estado = EstadosProducto.Activo
                },
                new Producto
                {
                    Id = 5, Nombre = "Teclado Mecánico RGB Ducky",
                    Descripcion = "Switches mecánicos, retroiluminación RGB personalizable",
                    Precio = 149.00m, CategoriaId = 5,
                    Imagen = "productos/teclado.jpg", Stock = 5, Estado = EstadosProducto.Oferta
                },
                new Producto
                {
                    Id = 6, Nombre = "Monitor UltraWide 4K 34\"",
                    Descripcion = "Panel IPS, 144Hz, ideal para trabajo y gaming",
                    Precio = 599.00m, CategoriaId = 6,
                    Imagen = "productos/monitor.jpg", Stock = 0, Estado = EstadosProducto.Agotado
                },
                new Producto
                {
                    Id = 7, Nombre = "Webcam 4K StreamPro",
                    Descripcion = "Grabación 4K a 30fps, enfoque automático, micrófono dual",
                    Precio = 129.00m, CategoriaId = 5,
                    Imagen = "productos/webcam.jpg", Stock = 18, Estado = EstadosProducto.Activo
                },
                new Producto
                {
                    Id = 8, Nombre = "Mouse Gaming Ultraligero",
                    Descripcion = "63 gramos, sensor óptico 26000 DPI, cable paracord",
                    Precio = 79.00m, CategoriaId = 5,
                    Imagen = "productos/mouse.jpg", Stock = 42, Estado = EstadosProducto.Activo
                }
            };
        }
    }
}
