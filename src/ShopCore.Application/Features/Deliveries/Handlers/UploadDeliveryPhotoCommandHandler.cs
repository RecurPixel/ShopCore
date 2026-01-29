namespace ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;

public class UploadDeliveryPhotoCommandHandler : IRequestHandler<UploadDeliveryPhotoCommand, string>
{
    public Task<string> Handle(
        UploadDeliveryPhotoCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get delivery by id
        // 2. Verify current user is delivery person
        // 3. Validate image file (format, size, dimensions)
        // 4. Upload image to cloud storage (Azure Blob, S3, etc.)
        // 5. Save image URL/path to database
        // 6. Update delivery status if all proofs collected
        // 7. Return image URL or file id
        return Task.FromResult(string.Empty);
    }
}
