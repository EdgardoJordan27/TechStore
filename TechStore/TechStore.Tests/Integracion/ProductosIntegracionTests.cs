using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TechStore.Controllers;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;
using TechStore.Tests.Utilidades;
using TechStore.ViewModels;

namespace TechStore.Tests.Integracion
{
    // Prueba de integración: el controlador, los servicios reales y Entity Framework Core
    // trabajan juntos sobre una base de datos en memoria (sin Moq).
    public class ProductosIntegracionTests
    {
        private static ProductosController CrearControlador(TechStoreDbContext contexto)
        {
            var http = new DefaultHttpContext();
            return new ProductosController(
                new ProductoService(contexto),
                new CategoriaService(contexto),
                NullLogger<ProductosController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = http },
                TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>())
            };
        }

        private static async Task<TechStoreDbContext> CrearContextoConCategoriaAsync()
        {
            var contexto = new TechStoreDbContext(BaseDeDatosPrueba.CrearOpciones());
            contexto.Categorias.Add(BaseDeDatosPrueba.NuevaCategoria());
            await contexto.SaveChangesAsync();
            return contexto;
        }

        private static ProductoFormViewModel Formulario(string nombre, int stock)
        {
            return new ProductoFormViewModel
            {
                Nombre = nombre,
                Descripcion = "Producto creado en la prueba de integración",
                Precio = 25.50m,
                CategoriaId = 1,
                Stock = stock,
                Estado = EstadosProducto.Nuevo
            };
        }

        [Fact]
        public async Task AgregarProducto_ConServiciosReales_SeGuardaYApareceEnElListado()
        {
            await using var contexto = await CrearContextoConCategoriaAsync();
            var controlador = CrearControlador(contexto);

            var respuesta = await controlador.Agregar(Formulario("Hub USB-C", 15));
            var listado = await controlador.Index();

            Assert.IsType<RedirectToActionResult>(respuesta);

            var vista = Assert.IsType<ViewResult>(listado);
            var productos = Assert.IsAssignableFrom<List<Producto>>(vista.Model);
            var guardado = Assert.Single(productos);
            Assert.Equal("Hub USB-C", guardado.Nombre);
            Assert.Equal(15, guardado.Stock);
            Assert.Equal("Periféricos", guardado.Categoria?.Nombre);
        }

        [Fact]
        public async Task FlujoCompleto_AgregarEditarYEliminar_DejaLaBaseDeDatosCoherente()
        {
            await using var contexto = await CrearContextoConCategoriaAsync();
            var controlador = CrearControlador(contexto);

            await controlador.Agregar(Formulario("Cable HDMI", 40));
            var creado = await contexto.Productos.SingleAsync();

            var cambios = Formulario("Cable HDMI 2.1", 25);
            cambios.Id = creado.Id;
            var respuestaEdicion = await controlador.Editar(creado.Id, cambios);

            Assert.IsType<RedirectToActionResult>(respuestaEdicion);
            var editado = await contexto.Productos.AsNoTracking().SingleAsync();
            Assert.Equal("Cable HDMI 2.1", editado.Nombre);
            Assert.Equal(25, editado.Stock);

            var respuestaEliminar = await controlador.EliminarConfirmado(creado.Id);

            Assert.IsType<RedirectToActionResult>(respuestaEliminar);
            Assert.Empty(await contexto.Productos.ToListAsync());
        }

        [Fact]
        public async Task AgregarProducto_ConCategoriaInexistente_NoSeGuarda()
        {
            await using var contexto = await CrearContextoConCategoriaAsync();
            var controlador = CrearControlador(contexto);
            var modelo = Formulario("Producto huérfano", 5);
            modelo.CategoriaId = 500;

            var respuesta = await controlador.Agregar(modelo);

            Assert.IsType<ViewResult>(respuesta);
            Assert.False(controlador.ModelState.IsValid);
            Assert.Empty(await contexto.Productos.ToListAsync());
        }
    }
}
