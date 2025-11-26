using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrbanBarberAPI.Data;
using UrbanBarberAPI.DTOs;
using UrbanBarberAPI.Models;

namespace UrbanBarberAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool success, string message, string token, Usuario usuario)> Login(LoginDto loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username ||
                                         u.Correo == loginDto.Username ||
                                         u.Celular == loginDto.Username);

            if (usuario == null)
            {
                return (false, "Usuario no encontrado", null, null);
            }

            bool passwordValido = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash);

            if (!passwordValido)
            {
                return (false, "Contraseña incorrecta", null, null);
            }

            var token = GenerateJwtToken(usuario);

            return (true, "Login exitoso", token, usuario);
        }

        public async Task<(bool success, string message, Usuario usuario)> Register(RegisterDto registerDto)
        {
            // Validar username único
            var existeUsername = await _context.Usuarios.AnyAsync(u => u.Username == registerDto.Username);
            if (existeUsername)
            {
                return (false, "El username ya está registrado", null);
            }

            // Validar correo único
            var existeCorreo = await _context.Usuarios.AnyAsync(u => u.Correo == registerDto.Correo);
            if (existeCorreo)
            {
                return (false, "El correo ya está registrado", null);
            }

            // Validar celular único
            var existeCelular = await _context.Usuarios.AnyAsync(u => u.Celular == registerDto.Celular);
            if (existeCelular)
            {
                return (false, "El celular ya está registrado", null);
            }

            var nuevoUsuario = new Usuario
            {
                Username = registerDto.Username,
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                Correo = registerDto.Correo,
                Celular = registerDto.Celular,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Rol = "cliente",
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return (true, "Usuario registrado exitosamente", nuevoUsuario);
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}