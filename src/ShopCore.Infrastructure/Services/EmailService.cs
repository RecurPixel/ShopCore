using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopCore.Application.Common.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ShopCore.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var emailSettings = _configuration.GetSection("EmailSettings");
        _smtpHost = emailSettings["SmtpHost"] ?? throw new InvalidOperationException("SMTP Host not configured");
        _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        _smtpUsername = emailSettings["SmtpUsername"] ?? throw new InvalidOperationException("SMTP Username not configured");
        _smtpPassword = emailSettings["SmtpPassword"] ?? throw new InvalidOperationException("SMTP Password not configured");
        _fromEmail = emailSettings["FromEmail"] ?? throw new InvalidOperationException("From Email not configured");
        _fromName = emailSettings["FromName"] ?? "ShopCore";
        _enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        await SendEmailAsync(to, subject, body, null, isHtml);
    }

    public async Task SendEmailAsync(string to, string subject, string body, string? from, bool isHtml = true)
    {
        try
        {
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from ?? _fromEmail, _fromName);
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtml;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.EnableSsl = _enableSsl;

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_fromEmail, _fromName);

            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(recipient);
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtml;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.EnableSsl = _enableSsl;

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Bulk email sent successfully to {Count} recipients", recipients.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send bulk email to {Count} recipients", recipients.Count);
            throw;
        }
    }

    public async Task SendEmailWithAttachmentAsync(
        string to,
        string subject,
        string body,
        string attachmentPath,
        bool isHtml = true)
    {
        try
        {
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_fromEmail, _fromName);
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtml;

            if (File.Exists(attachmentPath))
            {
                var attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);
            }
            else
            {
                _logger.LogWarning("Attachment file not found: {Path}", attachmentPath);
            }

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.EnableSsl = _enableSsl;

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email with attachment sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
            throw;
        }
    }

    public async Task SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "Welcome to ShopCore!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Welcome to ShopCore, {userName}!</h2>
                <p>Thank you for registering with us.</p>
                <p>Get started by exploring our products and services.</p>
                <p>Best regards,<br/>The ShopCore Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal total)
    {
        var subject = $"Order Confirmation - {orderNumber}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Order Confirmed!</h2>
                <p>Your order <strong>{orderNumber}</strong> has been confirmed.</p>
                <p>Total Amount: ₹{total:N2}</p>
                <p>You will receive updates as your order is processed.</p>
                <p>Thank you for shopping with ShopCore!</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken, string resetUrl)
    {
        var subject = "Reset Your Password";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Password Reset Request</h2>
                <p>You requested to reset your password.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetUrl}?token={resetToken}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request this, please ignore this email.</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendEmailVerificationAsync(string to, string verificationToken, string verificationUrl)
    {
        var subject = "Verify Your Email Address";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Verify Your Email</h2>
                <p>Please verify your email address to activate your account.</p>
                <p><a href='{verificationUrl}?token={verificationToken}' style='background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
                <p>This link will expire in 24 hours.</p>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] pdfBytes)
    {
        var subject = $"Invoice - {invoiceNumber}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Your Invoice</h2>
                <p>Please find your invoice attached.</p>
                <p>Invoice Number: <strong>{invoiceNumber}</strong></p>
                <p>Thank you for your business!</p>
            </body>
            </html>
        ";

        // Create temp file for attachment
        var tempPath = Path.Combine(Path.GetTempPath(), $"{invoiceNumber}.pdf");
        await File.WriteAllBytesAsync(tempPath, pdfBytes);

        try
        {
            await SendEmailWithAttachmentAsync(to, subject, body, tempPath);
        }
        finally
        {
            // Clean up temp file
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}