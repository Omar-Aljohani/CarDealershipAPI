using CarDealershipAPI.Data;
using CarDealershipAPI.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace CarDealershipAPI.Services;

public class OtpService
{
    private readonly AppDbContext _db;
    private readonly ILogger<OtpService> _logger;
    public OtpService(AppDbContext db, ILogger<OtpService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task GenerateAndSendOtpAsync(Guid? userId, string email, string action)
    {
        // generate 6-digit OTP
        var rng = Random.Shared;
        var otp = rng.Next(100000, 999999).ToString();
        var hashed = BCrypt.Net.BCrypt.HashPassword(otp);

        var entry = new OtpEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            OtpHash = hashed,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            Used = false
        };

        _db.OtpEntries.Add(entry);
        await _db.SaveChangesAsync();

        _logger.LogInformation("[SIMULATED OTP DELIVERY] Email: {Email}, Action:{ Action}, OTP: { Otp} ", email, action, otp);

    }

    public async Task<(bool success, string? otpToken)> ValidateOtpAsync(string email, string action, string otp)
    {
        // find most recent non-used entry for action
        var entry = await _db.OtpEntries
        .Where(e => e.Action == action && !e.Used && e.ExpiresAt >=
        DateTime.UtcNow)
        .OrderByDescending(e => e.ExpiresAt)
        .FirstOrDefaultAsync();
        if (entry == null) return (false, null);
        if (!BCrypt.Net.BCrypt.Verify(otp, entry.OtpHash)) return (false, null);
        // mark used and generate token
        entry.Used = true;
        entry.OtpToken = Guid.NewGuid().ToString();
        entry.TokenExpiresAt = DateTime.UtcNow.AddMinutes(5);
        await _db.SaveChangesAsync();

        return (true, entry.OtpToken);
    }
    public async Task<bool> ValidateOtpTokenForActionAsync(Guid userId, string action, string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        var entry = await _db.OtpEntries.Where(e => e.UserId == userId && e.Action == action && e.OtpToken
        == token && e.TokenExpiresAt >= DateTime.UtcNow).FirstOrDefaultAsync();
        return entry != null;
    }


}