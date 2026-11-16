using System.ComponentModel.DataAnnotations;

namespace TechStore.ViewModels
{
    public class CategoriaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 300 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un ícono.")]
        [Display(Name = "Ícono")]
        public string Icono { get; set; } = string.Empty;
    }
}
