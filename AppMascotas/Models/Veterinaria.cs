using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppMascotas.Models
{
    public class Veterinaria : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre de la Veterinaria")]
        public string NombreVeterinaria { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(50)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        // Relaciones
        public virtual ICollection<Dueno> Duenos { get; set; } = new List<Dueno>();
        public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
