namespace ShopCore.Application.Addresses.Commands.SetDefaultAddress;

public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SetDefaultAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(SetDefaultAddressCommand request, CancellationToken ct)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == _currentUser.UserId && !a.IsDeleted, ct);

        if (address == null)
            throw new NotFoundException("Address", request.Id);

        // Unset all other defaults
        var existingDefaults = await _context.Addresses
            .Where(a => a.UserId == _currentUser.UserId && a.IsDefault && a.Id != request.Id)
            .ToListAsync(ct);

        foreach (var addr in existingDefaults)
        {
            addr.IsDefault = false;
        }

        address.IsDefault = true;

        await _context.SaveChangesAsync(ct);
    }
}
