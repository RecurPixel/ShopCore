namespace ShopCore.Application.Vendors.Commands.UploadVendorLogo;

public record UploadVendorLogoCommand(IFile AvatarFile) : IRequest<string>;
