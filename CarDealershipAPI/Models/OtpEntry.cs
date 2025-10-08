namespace CarDealershipAPI.Models
{
    public enum OTPAction { Login, Register, PurchaseRequest, UpdateVehicle}
    public class OtpEntry
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public OTPAction Action { get; set; } = OTPAction.Login; 
    public string OtpHash { get; set; } = string.Empty; // hashed OTP code
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;
        
public string? OtpToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
    }
}