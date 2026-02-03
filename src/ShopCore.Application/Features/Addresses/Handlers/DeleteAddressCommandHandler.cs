namespace ShopCore.Application.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == _currentUser.UserId && !a.IsDeleted, ct);

        if (address == null)
            throw new NotFoundException("Address", request.Id);

        // Soft delete
        address.IsDeleted = true;

        // If this was the default, make another address default
        if (address.IsDefault)
        {
            var anotherAddress = await _context.Addresses
                .Where(a => a.UserId == _currentUser.UserId && !a.IsDeleted && a.Id != request.Id)
                .FirstOrDefaultAsync(ct);

            if (anotherAddress != null)
            {
                anotherAddress.IsDefault = true;
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}
