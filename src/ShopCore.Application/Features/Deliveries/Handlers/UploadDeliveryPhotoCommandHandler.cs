namespace ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;

public class UploadDeliveryPhotoCommandHandler : IRequestHandler<UploadDeliveryPhotoCommand, string>
{
    public Task<string> Handle(
        UploadDeliveryPhotoCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}
