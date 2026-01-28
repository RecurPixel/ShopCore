using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;

public record UploadDeliveryPhotoCommand(
    int DeliveryId,
    IFile Photo
) : IRequest<string>;
