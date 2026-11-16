using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Tests.Utilidades
{
    // Crea contextos con una base de datos en memoria vacía (sin los datos iniciales),
    // de modo que cada prueba controla exactamente los registros que usa.
    public static class BaseDeDatosPrueba
    {
        public static DbContextOptions<TechStoreDbContext> CrearOpciones(string? nombre = null)
        {
            return new DbContextOptionsBuilder<TechStoreDbContext>()
                .UseInMemoryDatabase(nombre ?? Guid.NewGuid().ToString())
                .Options;
        }

        public static Categoria NuevaCategoria(int id = 1, string nombre = "Periféricos")
        {
            return new Categoria
            {
                Id = id,
                Nombre = nombre,
                Descripcion = "Categoría de prueba",
                Icono = IconosCategoria.Predeterminado
            };
        }

        public static Producto NuevoProducto(int id, string nombre, int categoriaId = 1, int stock = 10, string estado = EstadosProducto.Activo)
        {
            return new Producto
            {
                Id = id,
                Nombre = nombre,
                Descripcion = "Descripción de " + nombre,
                Precio = 100m,
                Stock = stock,
                Estado = estado,
                CategoriaId = categoriaId
            };
        }
    }
}
