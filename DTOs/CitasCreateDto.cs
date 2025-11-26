namespace UrbanBarberAPI.DTOs
{
    public class CitaCreateDto
    {
        public int BarberoId { get; set; }
        public int ServicioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Notas { get; set; }
    }
}