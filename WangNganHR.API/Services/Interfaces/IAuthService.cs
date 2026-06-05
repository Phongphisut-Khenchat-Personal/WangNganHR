using WangNganHR.API.DTOs.Auth;

namespace WangNganHR.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}