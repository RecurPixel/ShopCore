namespace ShopCore.Application.Vendors.Commands.ActivateVendor;

public record ActivateVendorCommand(int VendorId) : IRequest;
