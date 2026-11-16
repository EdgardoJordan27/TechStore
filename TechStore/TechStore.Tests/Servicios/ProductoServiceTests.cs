using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;
using TechStore.Tests.Utilidades;

namespace TechStore.Tests.Servicios
{
    public class ProductoServiceTests
    {
        // Prepara una base en memoria con una categoría y los productos indicados.
        private static async Task<DbContextOptions<TechStoreDbContext>> PrepararAsync(params Producto[] productos)
        {
            var opciones = BaseDeDatosPrueba.CrearOpciones();

            await using var contexto = new TechStoreDbContext(opciones);
            contexto.Categorias.Add(BaseDeDatosPrueba.NuevaCategoria());
            contexto.Productos.AddRange(productos);
            await contexto.SaveChangesAsync();

            return opciones;
        }

        [Fact]
        public async Task ObtenerTodosAsync_ConProductos_DevuelveTodosOrdenadosPorIdDescendente()
        {
            var opciones = await PrepararAsync(
                BaseDeDatosPrueba.NuevoProducto(1, "Mouse"),
                BaseDeDatosPrueba.NuevoProducto(2, "Teclado"),
                BaseDeDatosPrueba.NuevoProducto(3, "Monitor"));

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.ObtenerTodosAsync();

            Assert.Equal(3, resultado.Count);
            Assert.Equal(3, resultado[0].Id);
            Assert.Equal(1, resultado[2].Id);
            Assert.All(resultado, p => Assert.NotNull(p.Categoria));
        }

        [Fact]
        public async Task ObtenerTodosAsync_SinProductos_DevuelveListaVacia()
        {
            var opciones = await PrepararAsync();

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.ObtenerTodosAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_ProductoExistente_DevuelveElProductoConSuCategoria()
        {
            var opciones = await PrepararAsync(BaseDeDatosPrueba.NuevoProducto(5, "Webcam"));

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.ObtenerPorIdAsync(5);

            Assert.NotNull(resultado);
            Assert.Equal("Webcam", resultado!.Nombre);
            Assert.Equal("Periféricos", resultado.Categoria?.Nombre);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public async Task ObtenerPorIdAsync_IdInexistente_DevuelveNull(int id)
        {
            var opciones = await PrepararAsync(BaseDeDatosPrueba.NuevoProducto(1, "Mouse"));

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.ObtenerPorIdAsync(id);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AgregarAsync_ProductoValido_QuedaGuardadoEnLaBaseDeDatos()
        {
            var opciones = await PrepararAsync();
            var nuevo = BaseDeDatosPrueba.NuevoProducto(10, "Audífonos");

            await using (var contexto = new TechStoreDbContext(opciones))
            {
                var servicio = new ProductoService(contexto);
                await servicio.AgregarAsync(nuevo);
            }

            await using var verificacion = new TechStoreDbContext(opciones);
            var guardado = await verificacion.Productos.FindAsync(10);

            Assert.NotNull(guardado);
            Assert.Equal("Audífonos", guardado!.Nombre);
            Assert.Equal(1, await verificacion.Productos.CountAsync());
        }

        [Fact]
        public async Task EditarAsync_ProductoExistente_ActualizaLosDatosYDevuelveTrue()
        {
            var opciones = await PrepararAsync(BaseDeDatosPrueba.NuevoProducto(1, "Mouse", stock: 10));

            var cambios = BaseDeDatosPrueba.NuevoProducto(1, "Mouse Pro", stock: 3, estado: EstadosProducto.Oferta);
            cambios.Precio = 59.90m;

            bool resultado;
            await using (var contexto = new TechStoreDbContext(opciones))
            {
                var servicio = new ProductoService(contexto);
                resultado = await servicio.EditarAsync(cambios);
            }

            Assert.True(resultado);

            await using var verificacion = new TechStoreDbContext(opciones);
            var actualizado = await verificacion.Productos.FindAsync(1);

            Assert.Equal("Mouse Pro", actualizado!.Nombre);
            Assert.Equal(59.90m, actualizado.Precio);
            Assert.Equal(3, actualizado.Stock);
            Assert.Equal(EstadosProducto.Oferta, actualizado.Estado);
        }

        [Fact]
        public async Task EditarAsync_ProductoInexistente_DevuelveFalse()
        {
            var opciones = await PrepararAsync(BaseDeDatosPrueba.NuevoProducto(1, "Mouse"));

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.EditarAsync(BaseDeDatosPrueba.NuevoProducto(99, "Fantasma"));

            Assert.False(resultado);
        }

        [Fact]
        public async Task EliminarAsync_ProductoExistente_LoQuitaDeLaBaseDeDatos()
        {
            var opciones = await PrepararAsync(
                BaseDeDatosPrueba.NuevoProducto(1, "Mouse"),
                BaseDeDatosPrueba.NuevoProducto(2, "Teclado"));

            bool resultado;
            await using (var contexto = new TechStoreDbContext(opciones))
            {
                var servicio = new ProductoService(contexto);
                resultado = await servicio.EliminarAsync(1);
            }

            Assert.True(resultado);

            await using var verificacion = new TechStoreDbContext(opciones);
            Assert.Null(await verificacion.Productos.FindAsync(1));
            Assert.Equal(1, await verificacion.Productos.CountAsync());
        }

        [Fact]
        public async Task EliminarAsync_ProductoInexistente_DevuelveFalseYNoModificaNada()
        {
            var opciones = await PrepararAsync(BaseDeDatosPrueba.NuevoProducto(1, "Mouse"));

            await using var contexto = new TechStoreDbContext(opciones);
            var servicio = new ProductoService(contexto);

            var resultado = await servicio.EliminarAsync(999);

            Assert.False(resultado);
            Assert.Equal(1, await contexto.Productos.CountAsync());
        }
    }
}
