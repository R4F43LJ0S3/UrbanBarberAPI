using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanBarberAPI.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CitaId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "pendiente";

        public DateTime? FechaPago { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string TransaccionId { get; set; }

        [ForeignKey("CitaId")]
        public virtual Cita Cita { get; set; }
    }
}