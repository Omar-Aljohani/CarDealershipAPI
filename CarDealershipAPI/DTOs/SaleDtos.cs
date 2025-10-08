namespace CarDealershipAPI.DTOs
{
    public record PurchaseRequestDto(Guid VehicleId);
    public record AddSaleDto(Guid VehicleId, Guid CustomerId, decimal Price);
    public record ProcessSaleDto(Guid SaleID);
}
