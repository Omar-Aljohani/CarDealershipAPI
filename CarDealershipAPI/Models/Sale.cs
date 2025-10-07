namespace CarDealershipAPI.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid BuyerId { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public decimal Price { get; set; }
        // navigation
        public Vehicle? Vehicle { get; set; }
        public User? Buyer { get; set; }
    }
}
