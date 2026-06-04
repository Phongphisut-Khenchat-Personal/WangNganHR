using JanomeHR.API.DTOs.Auth;
using JanomeHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JanomeHR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { message = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง" });

        return Ok(result);
    }
}