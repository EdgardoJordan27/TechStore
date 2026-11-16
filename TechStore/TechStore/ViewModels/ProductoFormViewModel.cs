using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechStore.ViewModels
{
    public class ProductoFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(400, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 400 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal? Precio { get; set; }

        [Required(ErrorMessage = "Seleccione una categoría.")]
        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100000.")]
        public int? Stock { get; set; }

        [Required(ErrorMessage = "Seleccione un estado.")]
        public string Estado { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La ruta de la imagen no puede superar los 200 caracteres.")]
        public string? Imagen { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
    }
}
