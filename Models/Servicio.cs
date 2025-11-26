using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanBarberAPI.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Range(15, 120)]
        public int Duracion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public bool Disponible { get; set; } = true;

        public int Popularidad { get; set; } = 0;

        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}