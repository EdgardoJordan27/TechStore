using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TechStore.Models;
using TechStore.Services.Interfaces;

namespace TechStore.Services
{
    // Genera los reportes de productos en memoria: PDF con QuestPDF y Excel con ClosedXML.
    public class ReporteService : IReporteService
    {
        private const string NombreTienda = "TechStore";

        public byte[] GenerarPdfProductos(IEnumerable<Producto> productos)
        {
            var lista = productos.OrderBy(p => p.Nombre).ToList();
            var fecha = DateTime.Now;

            var documento = Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4.Landscape());
                    pagina.Margin(30);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(9));

                    pagina.Header().Column(columna =>
                    {
                        columna.Item().Text(NombreTienda).FontSize(20).Bold();
                        columna.Item().Text("Reporte de productos").FontSize(13);
                        columna.Item().Text($"Generado el {fecha:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                        columna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });

                    pagina.Content().PaddingVertical(10).Column(columna =>
                    {
                        columna.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.ConstantColumn(30);
                                columnas.RelativeColumn(2);
                                columnas.RelativeColumn(3);
                                columnas.ConstantColumn(60);
                                columnas.RelativeColumn(2);
                                columnas.ConstantColumn(40);
                                columnas.ConstantColumn(60);
                            });

                            tabla.Header(encabezado =>
                            {
                                var titulos = new[] { "ID", "Producto", "Descripción", "Precio", "Categoría", "Stock", "Estado" };
                                foreach (var titulo in titulos)
                                {
                                    encabezado.Cell().Element(CeldaEncabezado)
                                        .Text(titulo).Bold().FontColor(Colors.White);
                                }
                            });

                            foreach (var p in lista)
                            {
                                tabla.Cell().Element(CeldaDato).Text(p.Id.ToString());
                                tabla.Cell().Element(CeldaDato).Text(p.Nombre);
                                tabla.Cell().Element(CeldaDato).Text(p.Descripcion);
                                tabla.Cell().Element(CeldaDato).Text($"${p.Precio:N2}");
                                tabla.Cell().Element(CeldaDato).Text(p.Categoria?.Nombre ?? "Sin categoría");
                                tabla.Cell().Element(CeldaDato).Text(p.Stock.ToString());
                                tabla.Cell().Element(CeldaDato).Text(p.Estado);
                            }
                        });

                        columna.Item().PaddingTop(10).Text($"Total de productos: {lista.Count}").Bold();
                    });

                    pagina.Footer().AlignCenter().Text(texto =>
                    {
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GenerarExcelProductos(IEnumerable<Producto> productos)
        {
            var lista = productos.OrderBy(p => p.Id).ToList();
            const int filaEncabezado = 4;
            const int totalColumnas = 7;
            var ultimaFila = filaEncabezado + lista.Count;

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Productos");

            hoja.Cell(1, 1).Value = $"{NombreTienda} - Reporte de productos";
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 14;
            hoja.Cell(2, 1).Value = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";

            var titulos = new[] { "ID", "Nombre", "Descripción", "Precio", "Categoría", "Stock", "Estado" };
            for (var i = 0; i < titulos.Length; i++)
            {
                hoja.Cell(filaEncabezado, i + 1).Value = titulos[i];
            }

            var fila = filaEncabezado + 1;
            foreach (var p in lista)
            {
                hoja.Cell(fila, 1).Value = p.Id;
                hoja.Cell(fila, 2).Value = p.Nombre;
                hoja.Cell(fila, 3).Value = p.Descripcion;
                hoja.Cell(fila, 4).Value = p.Precio;
                hoja.Cell(fila, 4).Style.NumberFormat.Format = "$#,##0.00";
                hoja.Cell(fila, 5).Value = p.Categoria?.Nombre ?? "Sin categoría";
                hoja.Cell(fila, 6).Value = p.Stock;
                hoja.Cell(fila, 7).Value = p.Estado;
                fila++;
            }

            var encabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, totalColumnas);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#333333");
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var tabla = hoja.Range(filaEncabezado, 1, ultimaFila, totalColumnas);
            tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tabla.SetAutoFilter();

            hoja.Columns(1, totalColumnas).AdjustToContents(filaEncabezado, ultimaFila);
            if (hoja.Column(3).Width > 60)
            {
                hoja.Column(3).Width = 60;
            }

            hoja.SheetView.FreezeRows(filaEncabezado);

            using var memoria = new MemoryStream();
            libro.SaveAs(memoria);
            return memoria.ToArray();
        }

        private static IContainer CeldaEncabezado(IContainer contenedor)
        {
            return contenedor.Background(Colors.Grey.Darken3).Padding(5);
        }

        private static IContainer CeldaDato(IContainer contenedor)
        {
            return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }
}