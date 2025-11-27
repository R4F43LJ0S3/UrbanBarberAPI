namespace UrbanBarberAPI.DTOs
{
    public class CitaCreateDto
    {
        // Para usuarios autenticados (opcional)
        public int? UsuarioId { get; set; }

        // Para citas sin autenticación (requerido si no hay UsuarioId)
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }

        // Datos de la cita (siempre requeridos)
        public int BarberoId { get; set; }
        public int ServicioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Notas { get; set; }
    }
}