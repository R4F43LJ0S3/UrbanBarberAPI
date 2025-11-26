using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrbanBarberAPI.Data;
using UrbanBarberAPI.DTOs;
using UrbanBarberAPI.Services;

namespace UrbanBarberAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var resultado = await _authService.Register(registerDto);

            if (!resultado.success)
            {
                return BadRequest(new { message = resultado.message });
            }

            return Ok(new
            {
                message = resultado.message,
                usuario = new
                {
                    resultado.usuario.Id,
                    resultado.usuario.Username,
                    resultado.usuario.Nombre,
                    resultado.usuario.Apellido,
                    resultado.usuario.Correo,
                    resultado.usuario.Celular,
                    resultado.usuario.Rol
                }
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var resultado = await _authService.Login(loginDto);

            if (!resultado.success)
            {
                return Unauthorized(new { message = resultado.message });
            }

            return Ok(new
            {
                message = resultado.message,
                token = resultado.token,
                usuario = new
                {
                    resultado.usuario.Id,
                    resultado.usuario.Username,
                    resultado.usuario.Nombre,
                    resultado.usuario.Apellido,
                    resultado.usuario.Correo,
                    resultado.usuario.Celular,
                    resultado.usuario.Rol
                }
            });
        }

        // GET: api/auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                usuario.Id,
                usuario.Username,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Correo,
                usuario.Celular,
                usuario.Rol,
                usuario.FechaRegistro
            });
        }
    }
}
