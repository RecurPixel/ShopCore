namespace ShopCore.Application.Vendors.Commands.UpdateMyVendor;

public class UpdateMyVendorCommandHandler : IRequestHandler<UpdateMyVendorCommand>
{
    public Task Handle(UpdateMyVendorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
