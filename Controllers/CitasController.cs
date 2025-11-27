using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrbanBarberAPI.Data;
using UrbanBarberAPI.DTOs;
using UrbanBarberAPI.Models;

namespace UrbanBarberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las citas del usuario autenticado (o todas si es admin)
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMisCitas()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<Cita> query = _context.Citas
                .Include(c => c.Barbero)
                .Include(c => c.Servicio)
                .Include(c => c.Usuario);

            if (userRole != "admin")
            {
                query = query.Where(c => c.UsuarioId == userId);
            }

            var citas = await query
                .Select(c => new
                {
                    c.Id,
                    c.Fecha,
                    c.Hora,
                    c.Estado,
                    c.Pagado,
                    c.Notas,
                    Barbero = new
                    {
                        c.Barbero.Id,
                        c.Barbero.Nombre,
                        c.Barbero.Especialidad
                    },
                    Servicio = new
                    {
                        c.Servicio.Id,
                        c.Servicio.Nombre,
                        c.Servicio.Precio,
                        c.Servicio.Duracion
                    },
                    Usuario = new
                    {
                        c.Usuario.Id,
                        c.Usuario.Nombre,
                        c.Usuario.Apellido,
                        c.Usuario.Celular
                    }
                })
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return Ok(citas);
        }

        /// <summary>
        /// Obtiene una cita específica por ID
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCita(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var cita = await _context.Citas
                .Include(c => c.Barbero)
                .Include(c => c.Servicio)
                .Include(c => c.Usuario)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (cita == null)
            {
                return NotFound(new { message = "Cita no encontrada" });
            }

            if (cita.UsuarioId != userId && userRole != "admin")
            {
                return Forbid();
            }

            return Ok(new
            {
                cita.Id,
                cita.Fecha,
                cita.Hora,
                cita.Estado,
                cita.Pagado,
                cita.Notas,
                Barbero = new
                {
                    cita.Barbero.Id,
                    cita.Barbero.Nombre,
                    cita.Barbero.Especialidad
                },
                Servicio = new
                {
                    cita.Servicio.Id,
                    cita.Servicio.Nombre,
                    cita.Servicio.Precio,
                    cita.Servicio.Duracion
                }
            });
        }

        /// <summary>
        /// Crea una nueva cita (NO requiere autenticación)
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCita([FromBody] CitaCreateDto citaDto)
        {
            // Validar que el barbero existe
            var barbero = await _context.Barberos.FindAsync(citaDto.BarberoId);
            if (barbero == null || !barbero.Disponible)
            {
                return BadRequest(new { message = "Barbero no disponible" });
            }

            // Validar que el servicio existe
            var servicio = await _context.Servicios.FindAsync(citaDto.ServicioId);
            if (servicio == null || !servicio.Disponible)
            {
                return BadRequest(new { message = "Servicio no disponible" });
            }

            // Validar que la fecha no sea pasada
            if (citaDto.Fecha.Date < DateTime.Today)
            {
                return BadRequest(new { message = "La fecha no puede ser anterior a hoy" });
            }

            // Validar horario de atención
            if (citaDto.Hora.Hours < 7 || citaDto.Hora.Hours >= 22)
            {
                return BadRequest(new { message = "El horario de atención es de 7:00 AM a 10:00 PM" });
            }

            int usuarioId;

            // Si viene un UsuarioId, usarlo (usuario autenticado)
            if (citaDto.UsuarioId.HasValue && citaDto.UsuarioId.Value > 0)
            {
                usuarioId = citaDto.UsuarioId.Value;
            }
            else
            {
                // Validar que vengan los datos necesarios para crear usuario temporal
                if (string.IsNullOrWhiteSpace(citaDto.Nombre) ||
                    string.IsNullOrWhiteSpace(citaDto.Celular))
                {
                    return BadRequest(new { message = "Debe proporcionar nombre y celular" });
                }

                // Crear usuario temporal para citas sin autenticación
                var usuarioTemp = new Usuario
                {
                    Username = $"temp_{citaDto.Nombre.Replace(" ", "")}_{DateTime.UtcNow.Ticks}",
                    Nombre = citaDto.Nombre,
                    Apellido = "Temporal",
                    Correo = citaDto.Correo ?? $"temp{DateTime.UtcNow.Ticks}@temp.com",
                    Celular = citaDto.Celular,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("temp123"),
                    Rol = "cliente",
                    FechaRegistro = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuarioTemp);
                await _context.SaveChangesAsync();
                usuarioId = usuarioTemp.Id;
            }

            // Crear la cita
            var nuevaCita = new Cita
            {
                UsuarioId = usuarioId,
                BarberoId = citaDto.BarberoId,
                ServicioId = citaDto.ServicioId,
                Fecha = citaDto.Fecha,
                Hora = citaDto.Hora,
                Notas = citaDto.Notas ?? "",
                Estado = "pendiente",
                Pagado = false,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Citas.Add(nuevaCita);
            await _context.SaveChangesAsync();

            // Incrementar popularidad del servicio
            servicio.Popularidad++;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cita creada exitosamente",
                citaId = nuevaCita.Id
            });
        }

        /// <summary>
        /// Elimina una cita existente
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteCita(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound(new { message = "Cita no encontrada" });
            }

            if (cita.UsuarioId != userId && userRole != "admin")
            {
                return Forbid();
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cita eliminada exitosamente" });
        }

        /// <summary>
        /// Marca una cita como pagada
        /// </summary>
        [Authorize]
        [HttpPut("{id}/pagar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarcarComoPagada(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound(new { message = "Cita no encontrada" });
            }

            if (cita.UsuarioId != userId && userRole != "admin")
            {
                return Forbid();
            }

            cita.Pagado = true;
            cita.Estado = "confirmada";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cita marcada como pagada" });
        }
    }
}