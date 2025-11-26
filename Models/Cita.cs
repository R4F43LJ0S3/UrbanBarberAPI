using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanBarberAPI.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int BarberoId { get; set; }

        [Required]
        public int ServicioId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [StringLength(200)]
        public string Notas { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "pendiente";

        public bool Pagado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("BarberoId")]
        public virtual Barbero Barbero { get; set; }

        [ForeignKey("ServicioId")]
        public virtual Servicio Servicio { get; set; }

        public virtual Pago Pago { get; set; }
    }
}