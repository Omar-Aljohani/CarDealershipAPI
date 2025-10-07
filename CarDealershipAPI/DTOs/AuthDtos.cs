namespace CarDealershipAPI.DTOs
{
    public record RegisterRequest(string Name, string Email, string Password,
string Role);
    public record LoginRequest(string Email, string Password);
    public record RequestOtpDto(string Email, string Action);
    public record ValidateOtpDto(string Email, string Action, string Otp);
    public record AuthResponse(string Token, string Role);
}

