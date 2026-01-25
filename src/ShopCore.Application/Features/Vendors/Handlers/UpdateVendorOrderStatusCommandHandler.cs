namespace ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;

public class UpdateVendorOrderStatusCommandHandler : IRequestHandler<UpdateVendorOrderStatusCommand>
{
    public Task Handle(UpdateVendorOrderStatusCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
