using WangNganHR.API.Data;
using WangNganHR.API.DTOs.Auth;
using WangNganHR.API.Helpers;
using WangNganHR.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WangNganHR.API.Services;

public class AuthService(AppDbContext db, JwtHelper jwt) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

        if (user is null) return null;

        // TODO: เปลี่ยนเป็น BCrypt ภายหลัง
        if (user.PasswordHash != dto.Password) return null;

        user.LastLogin = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var token = jwt.GenerateToken(user);
        var expiry = DateTime.UtcNow.AddHours(8);

        return new LoginResponseDto(token, user.FullName, user.Role.ToString(), expiry);
    }
}