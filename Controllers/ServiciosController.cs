using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanBarberAPI.Data;

namespace UrbanBarberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/servicios
        [HttpGet]
        public async Task<IActionResult> GetServicios()
        {
            var servicios = await _context.Servicios
                .Where(s => s.Disponible)
                .Select(s => new
                {
                    s.Id,
                    s.Nombre,
                    s.Descripcion,
                    s.Duracion,
                    s.Precio,
                    s.Disponible,
                    s.Popularidad
                })
                .ToListAsync();

            return Ok(servicios);
        }

        // GET: api/servicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicio(int id)
        {
            var servicio = await _context.Servicios
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Nombre,
                    s.Descripcion,
                    s.Duracion,
                    s.Precio,
                    s.Disponible,
                    s.Popularidad
                })
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return NotFound(new { message = "Servicio no encontrado" });
            }

            return Ok(servicio);
        }
    }
}