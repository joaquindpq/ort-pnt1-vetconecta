using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppMascotas.Models
{
    public class Mascota
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre de la Mascota")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La especie es obligatoria")]
        [StringLength(50)]
        public string Especie { get; set; }

        [StringLength(50)]
        public string? Raza { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Sexo { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [Display(Name = "Peso (kg)")]
        [Range(0.1, 500, ErrorMessage = "El peso debe estar entre 0.1 y 500 kg")]
        public decimal? Peso { get; set; }

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Dueño")]
        public int DuenoId { get; set; }
        
        [ForeignKey("DuenoId")]
        public virtual Dueno? Dueno { get; set; }

        [Required]
        public string VeterinariaId { get; set; }
        
        [ForeignKey("VeterinariaId")]
        public virtual Veterinaria? Veterinaria { get; set; }

        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
