using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppMascotas.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias")]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Motivo de la Consulta")]
        public string Motivo { get; set; }

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }
        
        [ForeignKey("MascotaId")]
        public virtual Mascota? Mascota { get; set; }

        [Required]
        public string VeterinariaId { get; set; }
        
        [ForeignKey("VeterinariaId")]
        public virtual Veterinaria? Veterinaria { get; set; }

        [NotMapped]
        public string EstadoActual
        {
            get
            {
                if (Estado == "Cancelado")
                    return "Cancelado";

                if (FechaHora < DateTime.Now && Estado == "Pendiente")
                    return "Realizado";

                return Estado;
            }
        }
    }
}
