using System.ComponentModel.DataAnnotations;
namespace TechStore.Models
{
    public class ContactoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresá un correo válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio")]
        public string Asunto { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [MinLength(10, ErrorMessage = "El mensaje debe tener al menos 10 caracteres")]
        public string Mensaje { get; set; }
    }
}