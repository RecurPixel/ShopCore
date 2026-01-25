namespace ShopCore.Application.Vendors.Commands.ApproveVendor;

public class ApproveVendorCommandHandler : IRequestHandler<ApproveVendorCommand>
{
    public Task Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
