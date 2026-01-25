namespace ShopCore.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailVerificationAsync(string to, string verificationToken);
    Task SendPasswordResetAsync(string to, string resetToken);
    Task SendOrderConfirmationAsync(string to, int orderId);
    Task SendInvoiceAsync(string to, int invoiceId);
}
