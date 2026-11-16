namespace TechStore.Models
{
    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Vendedor = "Vendedor";
        public const string Cliente = "Cliente";

        // Combinaciones usadas en [Authorize(Roles = ...)]
        public const string Personal = Administrador + "," + Vendedor;

        public static readonly IReadOnlyList<string> Todos = new[]
        {
            Administrador, Vendedor, Cliente
        };
    }
}
