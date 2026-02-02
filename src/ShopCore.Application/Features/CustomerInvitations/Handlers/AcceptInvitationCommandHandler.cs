using ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class AcceptInvitationCommandHandler
    : IRequestHandler<AcceptInvitationCommand, InvitationAcceptedDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMediator _mediator;

    public AcceptInvitationCommandHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMediator mediator)
    {
        _context = context;
        _dateTime = dateTime;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mediator = mediator;
    }

    public async Task<InvitationAcceptedDto> Handle(AcceptInvitationCommand request, CancellationToken ct)
    {
        var invitation = await _context.CustomerInvitations
            .Include(i => i.Vendor)
            .FirstOrDefaultAsync(i => i.InvitationToken == request.InvitationToken, ct);

        if (invitation == null)
            throw new NotFoundException("Invitation not found");

        if (invitation.Status != InvitationStatus.Pending)
            throw new ValidationException("Invitation is no longer valid");

        if (invitation.ExpiresAt < _dateTime.UtcNow)
            throw new ValidationException("Invitation has expired");

        if (!request.AgreeToTerms)
            throw new ValidationException("You must agree to terms and conditions");

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == invitation.PhoneNumber, ct);

        User user;
        if (existingUser == null)
        {
            // Create new user
            user = new User
            {
                FirstName = invitation.CustomerName.Split(' ').FirstOrDefault() ?? invitation.CustomerName,
                LastName = invitation.CustomerName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = invitation.Email ?? $"{invitation.PhoneNumber}@temp.com",
                PhoneNumber = invitation.PhoneNumber,
                PasswordHash = !string.IsNullOrEmpty(request.Password)
                    ? _passwordHasher.HashPassword(request.Password)
                    : _passwordHasher.HashPassword(Guid.NewGuid().ToString()), // Temp password
                Role = UserRole.Customer,
                IsEmailVerified = false,
                IsPhoneVerified = true, // Assume verified via invitation
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
        }
        else
        {
            user = existingUser;
        }

        // Create address
        var address = new Address
        {
            UserId = user.Id,
            FullName = invitation.CustomerName,
            PhoneNumber = invitation.PhoneNumber,
            AddressLine1 = invitation.DeliveryAddress,
            City = "Unknown", // Extract from address if possible
            State = "Unknown",
            Country = "India",
            PinCode = invitation.Pincode,
            Landmark = invitation.Landmark,
            Type = AddressType.Home,
            IsDefault = true
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(ct);

        // Deserialize items
        var items = System.Text.Json.JsonSerializer.Deserialize<List<InvitationItemInput>>(invitation.ItemsJson);

        // Create subscription using existing CreateSubscriptionCommand
        var subscriptionItems = items.Select(i => new SubscriptionItemDto(i.ProductId, i.Quantity)).ToList();

        var createSubCommand = new CreateSubscriptionCommand(
            VendorId: invitation.VendorId,
            DeliveryAddressId: address.Id,
            Items: subscriptionItems,
            Frequency: invitation.Frequency,
            CustomFrequencyDays: null,
            StartDate: _dateTime.UtcNow.AddDays(1),
            PreferredDeliveryTime: invitation.PreferredDeliveryTime,
            BillingCycleDays: 30, // Default
            DepositAmount: invitation.DepositAmount
        );

        // Note: This would need ICurrentUserService to be set to the new user's context
        // For simplicity, directly create subscription here
        var subscription = await CreateSubscriptionDirectly(
            user.Id,
            invitation.VendorId,
            address.Id,
            subscriptionItems,
            invitation,
            ct);

        // Update invitation
        invitation.Status = InvitationStatus.Accepted;
        invitation.AcceptedAt = _dateTime.UtcNow;
        invitation.AcceptedByUserId = user.Id;

        await _context.SaveChangesAsync(ct);

        // Generate JWT token for auto-login
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = _dateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(ct);

        return new InvitationAcceptedDto
        {
            UserId = user.Id,
            SubscriptionId = subscription.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Message = "Subscription created successfully!"
        };
    }

    private async Task<Subscription> CreateSubscriptionDirectly(
        int userId,
        int vendorId,
        int addressId,
        List<SubscriptionItemDto> items,
        CustomerInvitation invitation,
        CancellationToken ct)
    {
        var subNumber = await GenerateSubscriptionNumberAsync(_dateTime.UtcNow.Date, ct);

        var subscription = new Subscription
        {
            UserId = userId,
            VendorId = vendorId,
            DeliveryAddressId = addressId,
            SubscriptionNumber = subNumber,
            Frequency = invitation.Frequency,
            StartDate = _dateTime.UtcNow.AddDays(1),
            NextDeliveryDate = _dateTime.UtcNow.AddDays(1),
            PreferredDeliveryTime = invitation.PreferredDeliveryTime,
            Status = SubscriptionStatus.Active,
            BillingCycleDays = 30,
            NextBillingDate = _dateTime.UtcNow.AddDays(30),
            DepositAmount = invitation.DepositAmount,
            DepositPaid = invitation.DepositAmount,
            DepositBalance = invitation.DepositAmount,
            CreditLimit = 1200
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync(ct);

        foreach (var item in items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            var subItem = new SubscriptionItem
            {
                SubscriptionId = subscription.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                IsRecurring = true
            };
            _context.SubscriptionItems.Add(subItem);
        }

        await _context.SaveChangesAsync(ct);
        return subscription;
    }

    private async Task<string> GenerateSubscriptionNumberAsync(DateTime date, CancellationToken ct)
    {
        var prefix = $"SUB-{date:yyyyMMdd}";
        var count = await _context.Subscriptions.Where(s => s.SubscriptionNumber.StartsWith(prefix)).CountAsync(ct);
        return $"{prefix}-{(count + 1):D4}";
    }
}
