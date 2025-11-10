using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppMascotas.Models
{
    public class Dueno
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(20)]
        [Display(Name = "DNI")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string VeterinariaId { get; set; }
        
        [ForeignKey("VeterinariaId")]
        public virtual Veterinaria? Veterinaria { get; set; }

        public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}
