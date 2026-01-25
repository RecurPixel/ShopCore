namespace ShopCore.Application.Products.Commands.UpdateProductStatus;

public class UpdateProductStatusCommandHandler : IRequestHandler<UpdateProductStatusCommand>
{
    public Task Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
