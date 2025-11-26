using System.ComponentModel.DataAnnotations;

namespace UrbanBarberAPI.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Correo { get; set; }

        [Required]
        [StringLength(10)]
        public string Celular { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "cliente";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}