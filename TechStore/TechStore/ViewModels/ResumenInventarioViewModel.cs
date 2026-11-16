namespace TechStore.ViewModels
{
    public class ResumenInventarioViewModel
    {
        public int TotalProductos { get; set; }
        public int Agotados { get; set; }
        public int StockBajo { get; set; }
        public decimal ValorInventario { get; set; }
    }
}
