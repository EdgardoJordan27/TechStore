namespace TechStore.Models
{
    public record OpcionIcono(string Clase, string Etiqueta);

    public static class IconosCategoria
    {
        public const string Predeterminado = "bi-box-seam";

        public static readonly IReadOnlyList<OpcionIcono> Disponibles = new List<OpcionIcono>
        {
            new OpcionIcono("bi-box-seam", "General"),
            new OpcionIcono("bi-laptop", "Laptop"),
            new OpcionIcono("bi-phone", "Teléfono"),
            new OpcionIcono("bi-headphones", "Audífonos"),
            new OpcionIcono("bi-smartwatch", "Reloj inteligente"),
            new OpcionIcono("bi-keyboard", "Teclado"),
            new OpcionIcono("bi-mouse", "Mouse"),
            new OpcionIcono("bi-display", "Monitor"),
            new OpcionIcono("bi-camera-video", "Cámara"),
            new OpcionIcono("bi-cpu", "Componentes")
        };
    }
}
