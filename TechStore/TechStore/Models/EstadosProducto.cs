namespace TechStore.Models
{
    public static class EstadosProducto
    {
        public const string Activo = "Activo";
        public const string Nuevo = "Nuevo";
        public const string Oferta = "Oferta";
        public const string Agotado = "Agotado";

        public static readonly IReadOnlyList<string> Todos = new[]
        {
            Activo, Nuevo, Oferta, Agotado
        };
    }
}
