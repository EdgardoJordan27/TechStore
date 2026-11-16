using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TechStore.Controllers;
using TechStore.Models;
using TechStore.Services.Interfaces;
using TechStore.Tests.Utilidades;
using TechStore.ViewModels;

namespace TechStore.Tests.Controladores
{
    // Pruebas unitarias del controlador: los servicios se reemplazan con Moq
    // para comprobar la lógica del controlador sin tocar la base de datos.
    public class ProductosControllerTests
    {
        private readonly Mock<IProductoService> _productos = new();
        private readonly Mock<ICategoriaService> _categorias = new();

        public ProductosControllerTests()
        {
            _categorias.Setup(c => c.ObtenerTodasAsync()).ReturnsAsync(new List<Categoria>());
            _categorias.Setup(c => c.ObtenerPorIdAsync(1)).ReturnsAsync(BaseDeDatosPrueba.NuevaCategoria());
        }

        private ProductosController CrearControlador(ILogger<ProductosController>? logger = null)
        {
            var contexto = new DefaultHttpContext();
            var controlador = new ProductosController(
                _productos.Object, _categorias.Object, logger ?? NullLogger<ProductosController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = contexto },
                TempData = new TempDataDictionary(contexto, Mock.Of<ITempDataProvider>())
            };
            return controlador;
        }

        private static ProductoFormViewModel FormularioValido()
        {
            return new ProductoFormViewModel
            {
                Nombre = "Teclado Gamer",
                Descripcion = "Teclado mecánico con luces",
                Precio = 49.90m,
                CategoriaId = 1,
                Stock = 10,
                Estado = EstadosProducto.Activo
            };
        }

        [Fact]
        public async Task Index_DevuelveLaVistaConLosProductosDelServicio()
        {
            var lista = new List<Producto> { BaseDeDatosPrueba.NuevoProducto(1, "Mouse") };
            _productos.Setup(s => s.ObtenerTodosAsync()).ReturnsAsync(lista);

            var resultado = await CrearControlador().Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Same(lista, vista.Model);
            _productos.Verify(s => s.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Editar_Get_ProductoInexistente_DevuelveNotFound()
        {
            _productos.Setup(s => s.ObtenerPorIdAsync(99)).ReturnsAsync((Producto?)null);

            var resultado = await CrearControlador().Editar(99);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Agregar_Post_ModeloValido_LlamaAlServicioYRedirigeAlListado()
        {
            var controlador = CrearControlador();

            var resultado = await controlador.Agregar(FormularioValido());

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            _productos.Verify(s => s.AgregarAsync(It.Is<Producto>(p =>
                p.Nombre == "Teclado Gamer" && p.Precio == 49.90m && p.Stock == 10)), Times.Once);
            Assert.NotNull(controlador.TempData["Mensaje"]);
        }

        [Fact]
        public async Task Agregar_Post_ModeloInvalido_NoLlamaAlServicioYVuelveAlFormulario()
        {
            var controlador = CrearControlador();
            controlador.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            var resultado = await controlador.Agregar(FormularioValido());

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.IsType<ProductoFormViewModel>(vista.Model);
            _productos.Verify(s => s.AgregarAsync(It.IsAny<Producto>()), Times.Never);
        }

        [Fact]
        public async Task Agregar_Post_CategoriaInexistente_AgregaErrorDeValidacion()
        {
            var modelo = FormularioValido();
            modelo.CategoriaId = 77;
            _categorias.Setup(c => c.ObtenerPorIdAsync(77)).ReturnsAsync((Categoria?)null);
            var controlador = CrearControlador();

            var resultado = await controlador.Agregar(modelo);

            Assert.IsType<ViewResult>(resultado);
            Assert.True(controlador.ModelState.ContainsKey(nameof(ProductoFormViewModel.CategoriaId)));
            _productos.Verify(s => s.AgregarAsync(It.IsAny<Producto>()), Times.Never);
        }

        [Fact]
        public async Task Agregar_Post_ErrorDeBaseDeDatos_RegistraElErrorYMuestraElFormulario()
        {
            _productos.Setup(s => s.AgregarAsync(It.IsAny<Producto>()))
                .ThrowsAsync(new DbUpdateException("fallo simulado"));
            var logger = new Mock<ILogger<ProductosController>>();
            var controlador = CrearControlador(logger.Object);

            var resultado = await controlador.Agregar(FormularioValido());

            Assert.IsType<ViewResult>(resultado);
            Assert.False(controlador.ModelState.IsValid);
            logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task Editar_Post_ProductoExistente_ActualizaYRedirige()
        {
            _productos.Setup(s => s.EditarAsync(It.IsAny<Producto>())).ReturnsAsync(true);
            var modelo = FormularioValido();
            modelo.Id = 4;

            var resultado = await CrearControlador().Editar(4, modelo);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            _productos.Verify(s => s.EditarAsync(It.Is<Producto>(p => p.Id == 4)), Times.Once);
        }

        [Fact]
        public async Task Editar_Post_IdDeRutaDistintoAlDelModelo_DevuelveNotFound()
        {
            var modelo = FormularioValido();
            modelo.Id = 4;

            var resultado = await CrearControlador().Editar(5, modelo);

            Assert.IsType<NotFoundResult>(resultado);
            _productos.Verify(s => s.EditarAsync(It.IsAny<Producto>()), Times.Never);
        }

        [Fact]
        public async Task Eliminar_Post_ProductoExistente_EliminaYRedirige()
        {
            _productos.Setup(s => s.ObtenerPorIdAsync(2)).ReturnsAsync(BaseDeDatosPrueba.NuevoProducto(2, "Monitor"));
            _productos.Setup(s => s.EliminarAsync(2)).ReturnsAsync(true);

            var resultado = await CrearControlador().EliminarConfirmado(2);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            _productos.Verify(s => s.EliminarAsync(2), Times.Once);
        }

        [Fact]
        public async Task Eliminar_Post_ProductoInexistente_DevuelveNotFoundSinEliminar()
        {
            _productos.Setup(s => s.ObtenerPorIdAsync(99)).ReturnsAsync((Producto?)null);

            var resultado = await CrearControlador().EliminarConfirmado(99);

            Assert.IsType<NotFoundResult>(resultado);
            _productos.Verify(s => s.EliminarAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
