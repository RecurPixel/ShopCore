namespace ShopCore.Application.Vendors.Commands.ApproveVendor;

public record ApproveVendorCommand(int VendorId) : IRequest;
