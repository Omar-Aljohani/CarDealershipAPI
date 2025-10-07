namespace CarDealershipAPI.DTOs
{
    public record VehicleCreateDto(string Make, string Model, int Year, decimal
Price, string? Color, int Mileage, string? Description);
    public record VehicleUpdateDto(string Make, string Model, int Year, decimal
    Price, string? Color, int Mileage, bool Available, string? Description);
}
