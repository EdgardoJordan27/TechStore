using System.Text;
using ClosedXML.Excel;
using QuestPDF.Infrastructure;
using TechStore.Models;
using TechStore.Services;
using TechStore.Tests.Utilidades;

namespace TechStore.Tests.Servicios
{
    public class ReporteServiceTests
    {
        public ReporteServiceTests()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private static List<Producto> ProductosDePrueba()
        {
            var categoria = BaseDeDatosPrueba.NuevaCategoria();
            var primero = BaseDeDatosPrueba.NuevoProducto(1, "Mouse");
            var segundo = BaseDeDatosPrueba.NuevoProducto(2, "Teclado", stock: 0, estado: EstadosProducto.Agotado);
            primero.Categoria = categoria;
            segundo.Categoria = categoria;
            return new List<Producto> { primero, segundo };
        }

        [Fact]
        public void GenerarPdfProductos_DevuelveUnArchivoPdfValido()
        {
            var servicio = new ReporteService();

            var bytes = servicio.GenerarPdfProductos(ProductosDePrueba());

            Assert.NotEmpty(bytes);
            Assert.Equal("%PDF", Encoding.ASCII.GetString(bytes, 0, 4));
        }

        [Fact]
        public void GenerarExcelProductos_IncluyeEncabezadosYUnaFilaPorProducto()
        {
            var servicio = new ReporteService();

            var bytes = servicio.GenerarExcelProductos(ProductosDePrueba());

            using var libro = new XLWorkbook(new MemoryStream(bytes));
            var hoja = libro.Worksheet(1);

            Assert.Equal("ID", hoja.Cell(4, 1).GetString());
            Assert.Equal("Estado", hoja.Cell(4, 7).GetString());
            Assert.Equal("Mouse", hoja.Cell(5, 2).GetString());
            Assert.Equal("Teclado", hoja.Cell(6, 2).GetString());
            Assert.Equal("Agotado", hoja.Cell(6, 7).GetString());
            Assert.True(hoja.Cell(7, 1).IsEmpty());
        }
    }
}
