using System.ComponentModel.DataAnnotations;

namespace RaymiMusic.AppWeb.Models
{
    public class UsuarioEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public Guid PlanSuscripcionId { get; set; }
    }
}
