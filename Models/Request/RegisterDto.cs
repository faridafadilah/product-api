namespace AuthApi.Models.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Password_Confirmation { get; set; } = string.Empty;
}
