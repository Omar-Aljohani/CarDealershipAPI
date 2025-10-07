namespace CarDealershipAPI.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string? Color { get; set; }
        public int Mileage { get; set; }
        public bool Available { get; set; } = true;
        public string? Description { get; set; }

    }
}