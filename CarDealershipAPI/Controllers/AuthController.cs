using CarDealershipAPI.Data;
using CarDealershipAPI.DTOs;
using CarDealershipAPI.Models;
using CarDealershipAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealershipAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly OtpService _otp;

        public AuthController(AppDbContext db, JwtService jwt, OtpService otp)
        {
            _db = db;
            _jwt = jwt;
            _otp = otp;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email.ToLower()))
                return Conflict(new { message = "Email already in use" });

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Email = req.Email.ToLower(),
                Role = req.Role?.ToLower() == "admin" ? Role.Admin : Role.Customer,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Generate OTP for registration
            // You don't need to enter this OTP anywhere, it is for demonstration. 
            await _otp.GenerateAndSendOtpAsync(user.Id, user.Email.ToLower(), OTPAction.Register);
            return Ok(new { message = "User created. Verify OTP sent(simulated)." });
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
            await _otp.GenerateAndSendOtpAsync(user?.Id, dto.Email.ToLower(), dto.Action);
            return Ok(new { message = "OTP generated (simulated delivery)." });
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpDto dto)
        {

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());
            var (success, otpToken) = await _otp.ValidateOtpAsync(dto.Email.ToLower(), dto.Action, dto.Otp);
            if (!success) return BadRequest(new { message = "Invalid or expired OTP" });

            // If action is Login/Register -> return JWT only when appropriate
            if (dto.Action == OTPAction.Login)
            {
                if (user == null) return BadRequest(new { message = "User not found" });
                var token = _jwt.GenerateToken(user);
                return Ok(new AuthResponse(token, user.Role.ToString()));
            }

            // For actions requiring OTP token, return otpToken to be used in X-OTP-Token header.
            return Ok(new { otpToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower());
            if (user == null) return Unauthorized(new {message = "Invalid credentials" });

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Unauthorized(new { message = "Invalid credentials" });

            // Instead of issuing JWT immediately, require OTP for sensitive login
            await _otp.GenerateAndSendOtpAsync(user.Id, user.Email.ToLower(), OTPAction.Login);
            return Ok(new { message = "OTP sent to proceed with login (simulated)." });
        }
    }
}
