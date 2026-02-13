using AuthApi.Models.Common;
using AuthApi.Models.DTOs;

namespace AuthApi.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<object>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<(string AuthToken, string RefreshToken)>> LoginAsync(LoginDto dto);
}
