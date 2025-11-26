using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanBarberAPI.Models
{
    public class Barbero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Especialidad { get; set; } = string.Empty;

        [StringLength(500)]
        public string Imagen { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Experiencia { get; set; } = string.Empty;

        [Range(1.0, 5.0)]
        [Column(TypeName = "decimal(3,1)")]
        public decimal Rating { get; set; } = 5.0m;

        public bool Disponible { get; set; } = true;

        public int CitasAtendidas { get; set; } = 0;

        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}