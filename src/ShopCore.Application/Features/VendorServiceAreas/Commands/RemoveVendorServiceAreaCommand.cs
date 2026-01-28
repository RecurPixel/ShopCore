namespace ShopCore.Application.VendorServiceAreas.Commands.RemoveVendorServiceArea;

public record RemoveVendorServiceAreaCommand(int ServiceAreaId) : IRequest<bool>;
