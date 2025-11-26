using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanBarberAPI.Data;

namespace UrbanBarberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarberosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BarberosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/barberos
        [HttpGet]
        public async Task<IActionResult> GetBarberos()
        {
            var barberos = await _context.Barberos
                .Where(b => b.Disponible)
                .Select(b => new
                {
                    b.Id,
                    b.Nombre,
                    b.Especialidad,
                    b.Imagen,
                    b.Experiencia,
                    b.Rating,
                    b.Disponible,
                    b.CitasAtendidas
                })
                .ToListAsync();

            return Ok(barberos);
        }

        // GET: api/barberos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBarbero(int id)
        {
            var barbero = await _context.Barberos
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.Nombre,
                    b.Especialidad,
                    b.Imagen,
                    b.Experiencia,
                    b.Rating,
                    b.Disponible,
                    b.CitasAtendidas
                })
                .FirstOrDefaultAsync();

            if (barbero == null)
            {
                return NotFound(new { message = "Barbero no encontrado" });
            }

            return Ok(barbero);
        }
    }
}