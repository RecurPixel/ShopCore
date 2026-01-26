namespace ShopCore.Application.Common.Interfaces;

public interface IEmailService
{
    // Base methods
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(string to, string subject, string body, string? from, bool isHtml = true);
    Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = true);

    // Template methods
    Task SendWelcomeEmailAsync(string to, string userName);
    Task SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal total);
    Task SendPasswordResetEmailAsync(string to, string resetToken, string resetUrl);
    Task SendEmailVerificationAsync(string to, string verificationToken, string verificationUrl);
    Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] pdfBytes);
}
