using Microsoft.EntityFrameworkCore;
using AuthApi.Application.Interfaces;
using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Models.DTOs;
using AuthApi.Services;
using AuthApi.Models.Common;

namespace AuthApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<ServiceResult<object>> RegisterAsync(RegisterDto dto)
    {
        if (dto.Password != dto.Password_Confirmation)
            return ServiceResult<object>.Fail("Password confirmation does not match.");

        var exists = await _context.Users.AnyAsync(x => x.Username == dto.Username);
        if (exists)
            return ServiceResult<object>.Fail("Username already exists.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return ServiceResult<object>.Ok(message: "User registered successfully.");
    }

    public async Task<ServiceResult<(string AuthToken, string RefreshToken)>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);
        if (user == null)
            return ServiceResult<(string, string)>.Fail("Invalid username or password.");

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid)
            return ServiceResult<(string, string)>.Fail("Invalid username or password.");

        var authToken = _tokenService.GenerateAuthToken(user.Id.ToString(), user.Username);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return ServiceResult<(string, string)>.Ok((authToken, refreshToken), "Login successful.");
    }
}
