using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrbanBarberAPI.Data;
using UrbanBarberAPI.DTOs;
using UrbanBarberAPI.Services;

namespace UrbanBarberAPI.Controllers
{
    /// <summary>
    /// Controlador de autenticación y gestión de usuarios
    /// </summary>
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

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="registerDto">Datos del usuario a registrar</param>
        /// <returns>Usuario creado exitosamente</returns>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     POST /api/auth/register
        ///     {
        ///        "username": "juanperez",
        ///        "nombre": "Juan",
        ///        "apellido": "Pérez",
        ///        "correo": "juan@example.com",
        ///        "celular": "3001234567",
        ///        "password": "MiPassword123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Usuario registrado exitosamente</response>
        /// <response code="400">Datos inválidos o usuario ya existe</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Inicia sesión y obtiene un token JWT
        /// </summary>
        /// <param name="loginDto">Credenciales de inicio de sesión</param>
        /// <returns>Token JWT y datos del usuario</returns>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     POST /api/auth/login
        ///     {
        ///        "username": "admin",
        ///        "password": "1234"
        ///     }
        ///
        /// El username puede ser: nombre de usuario, email o celular
        /// 
        /// **Credenciales de prueba:**
        /// - Username: admin
        /// - Password: 1234
        /// </remarks>
        /// <response code="200">Login exitoso - Retorna token JWT</response>
        /// <response code="401">Credenciales inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Obtiene el perfil del usuario autenticado
        /// </summary>
        /// <returns>Datos del perfil del usuario actual</returns>
        /// <remarks>
        /// **Requiere autenticación JWT**
        /// 
        /// Usa el botón "Authorize" arriba y pega tu token JWT:
        /// 
        ///     Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// </remarks>
        /// <response code="200">Perfil obtenido exitosamente</response>
        /// <response code="401">No autenticado - Token inválido o expirado</response>
        /// <response code="404">Usuario no encontrado</response>
        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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