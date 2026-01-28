using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Commands.CreateCustomerInvitation;

public record CreateCustomerInvitationCommand(
    string CustomerName,
    string PhoneNumber,
    string? Email,
    string DeliveryAddress,
    string Pincode,
    string? Landmark,
    List<InvitationItemInput> Items,
    SubscriptionFrequency Frequency,
    string PreferredDeliveryTime,
    decimal? DepositAmount,
    bool SendSms,
    bool SendWhatsApp,
    bool SendEmail
) : IRequest<CustomerInvitationDto>;
