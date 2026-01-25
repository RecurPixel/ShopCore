namespace ShopCore.Application.Vendors.Commands.SuspendVendor;

public record SuspendVendorCommand(int VendorId, string Reason) : IRequest;
