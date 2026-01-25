namespace ShopCore.Application.Vendors.Commands.SuspendVendor;

public class SuspendVendorCommandHandler : IRequestHandler<SuspendVendorCommand>
{
    public Task Handle(SuspendVendorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
