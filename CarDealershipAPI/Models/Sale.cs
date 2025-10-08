namespace CarDealershipAPI.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public decimal Price { get; set; }
        // navigation
        public Vehicle? Vehicle { get; set; }
        // navigation
        public User? User { get; set; }
    }
}
