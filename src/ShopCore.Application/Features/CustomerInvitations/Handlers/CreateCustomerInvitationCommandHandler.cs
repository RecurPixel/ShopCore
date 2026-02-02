using ShopCore.Application.CustomerInvitations.Commands.CreateCustomerInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;
using System.Text.Json;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class CreateCustomerInvitationCommandHandler
    : IRequestHandler<CreateCustomerInvitationCommand, CustomerInvitationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateCustomerInvitationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerInvitationDto> Handle(
        CreateCustomerInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        // Get product prices for the items
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.VendorId == vendor.Id)
            .ToListAsync(cancellationToken);

        var subscriptionItems = request.Items.Select(item =>
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId)
                ?? throw new NotFoundException(nameof(Product), item.ProductId);
            return new InvitationSubscriptionItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
        }).ToList();

        var invitation = new CustomerInvitation
        {
            VendorId = vendor.Id,
            InvitationToken = GenerateToken(),
            CustomerName = request.CustomerName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            DeliveryAddress = request.DeliveryAddress,
            Pincode = request.Pincode,
            SubscriptionItemsJson = JsonSerializer.Serialize(subscriptionItems),
            Frequency = request.Frequency,
            PreferredDeliveryTime = request.PreferredDeliveryTime,
            DepositAmount = request.DepositAmount,
            Status = InvitationStatus.Pending,
            SentAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.CustomerInvitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Send SMS/WhatsApp/Email based on request flags
        var invitationLink = $"https://app.shopcore.com/accept-invitation/{invitationToken}";

        // if (request.SendSms)
        // {
        //     await _smsService.SendSmsAsync(
        //         request.PhoneNumber,
        //         $"You're invited to subscribe! Click: {invitationLink}");
        // }

        // if (request.SendEmail && !string.IsNullOrEmpty(request.Email))
        // {
        //     await _emailService.SendInvitationEmailAsync(
        //         request.Email,
        //         request.CustomerName,
        //         invitationLink);
        // }

        // if (request.SendWhatsApp)
        // {
        //     // Send via WhatsApp (implementation depends on provider)
        //     // await _whatsAppService.SendMessageAsync(...);
        // }

        // Update status to Sent
        invitation.Status = InvitationStatus.Sent;
        await _context.SaveChangesAsync(cancellationToken);

        return new CustomerInvitationDto(
            invitation.Id,
            invitation.VendorId,
            vendor.BusinessName,
            invitation.CustomerName,
            invitation.PhoneNumber,
            invitation.Email,
            invitation.DeliveryAddress,
            invitation.Pincode,
            invitation.Status,
            invitation.SentAt,
            invitation.ExpiresAt,
            invitation.AcceptedAt
        );
    }

    private static string GenerateToken()
    {
        return Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
    }
}
