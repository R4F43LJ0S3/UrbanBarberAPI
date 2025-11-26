using UrbanBarberAPI.DTOs;
using UrbanBarberAPI.Models;

namespace UrbanBarberAPI.Services
{
    public interface IAuthService
    {
        Task<(bool success, string message, string token, Usuario usuario)> Login(LoginDto loginDto);
        Task<(bool success, string message, Usuario usuario)> Register(RegisterDto registerDto);
        string GenerateJwtToken(Usuario usuario);
    }
}
