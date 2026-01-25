namespace ShopCore.Application.Products.Commands.UploadProductImages;

public class UploadProductImagesCommandHandler : IRequestHandler<UploadProductImagesCommand>
{
    public Task Handle(UploadProductImagesCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
