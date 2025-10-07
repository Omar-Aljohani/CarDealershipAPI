namespace CarDealershipAPI.Models
{
    public class OtpEntry
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty; // e.g. Register, Login, PurchaseRequest, UpdateVehicle
    public string OtpHash { get; set; } = string.Empty; // hashed OTP code
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;
        // after validation we return an OtpToken (GUID) saved in this entry (for demo reuse)
public string? OtpToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
    }
}