namespace WangNganHR.API.DTOs.Auth;

public record LoginDto(string Username, string Password);

public record LoginResponseDto(
    string Token,
    string FullName,
    string Role,
    DateTime ExpiresAt
);