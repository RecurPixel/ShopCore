using ShopCore.Application.Common.Models;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

public class CreateInvoicePaymentIntentHandler
    : IRequestHandler<CreateInvoicePaymentIntentCommand, PaymentIntentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public CreateInvoicePaymentIntentHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentGatewayFactory gatewayFactory)
    {
        _context = context;
        _currentUser = currentUser;
        _gatewayFactory = gatewayFactory;
    }

    public async Task<PaymentIntentDto> Handle(
        CreateInvoicePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.User)
            .Include(i => i.Subscription)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            throw new NotFoundException("Invoice", request.InvoiceId);

        // Verify ownership
        if (invoice.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only pay your own invoices");

        // Verify invoice is payable
        if (invoice.Status == InvoiceStatus.Paid)
            throw new ValidationException("Invoice is already paid");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new ValidationException("Cannot pay cancelled invoice");

        // Calculate amount to pay (balance due)
        var amountToCharge = invoice.BalanceDue;
        if (amountToCharge <= 0)
            throw new ValidationException("Nothing to pay");

        // Get payment gateway (specified or default)
        var gateway = request.Gateway.HasValue
            ? _gatewayFactory.GetGateway(request.Gateway.Value)
            : _gatewayFactory.GetDefaultGateway();

        // Create payment via generic interface
        var paymentResult = await gateway.CreatePaymentAsync(new CreatePaymentRequest
        {
            Amount = amountToCharge,
            Currency = "INR",
            ReferenceId = invoice.Id,
            ReferenceType = PaymentReferenceType.Invoice,
            CustomerEmail = invoice.User?.Email,
            CustomerPhone = invoice.User?.PhoneNumber,
            CustomerName = $"{invoice.User?.FirstName} {invoice.User?.LastName}".Trim(),
            Description = $"Invoice #{invoice.InvoiceNumber}"
        }, cancellationToken);

        if (!paymentResult.Success)
            throw new ValidationException(paymentResult.ErrorMessage ?? "Failed to create payment");

        // Store the payment reference
        invoice.PaymentTransactionId = paymentResult.GatewayOrderId;
        invoice.PaymentGateway = gateway.GatewayType;

        await _context.SaveChangesAsync(cancellationToken);

        // Extract client secret from provider-specific data
        var clientSecret = GetClientDataValue(paymentResult.ClientData, "client_secret")
                        ?? GetClientDataValue(paymentResult.ClientData, "key_id")
                        ?? string.Empty;

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentResult.GatewayOrderId,
            ClientSecret = clientSecret,
            Amount = amountToCharge,
            Currency = paymentResult.Currency,
            Status = PaymentStatus.Pending.ToString(),
            Gateway = gateway.GatewayType.ToString(),
            GatewayOrderId = paymentResult.GatewayOrderId,
            ClientData = paymentResult.ClientData
        };
    }

    private static string? GetClientDataValue(IDictionary<string, string> data, string key)
    {
        return data.TryGetValue(key, out var value) ? value : null;
    }
}
