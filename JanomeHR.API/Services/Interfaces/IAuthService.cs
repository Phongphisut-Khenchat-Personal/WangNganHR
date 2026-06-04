using JanomeHR.API.DTOs.Auth;

namespace JanomeHR.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}