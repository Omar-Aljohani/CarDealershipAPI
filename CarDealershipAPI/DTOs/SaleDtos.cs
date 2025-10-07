namespace CarDealershipAPI.DTOs
{
    public record PurchaseRequestDto(Guid VehicleId);
    public record ProcessSaleDto(Guid VehicleId, Guid CustomerId, decimal Price);
}
