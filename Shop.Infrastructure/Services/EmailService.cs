using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Net;
using System.Net.Mail;

namespace Shop.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> emailOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetToken)
    {
        var linkBase = _settings.PasswordResetLinkBase.TrimEnd('/');
        var resetLink = string.IsNullOrWhiteSpace(linkBase)
            ? resetToken
            : $"{linkBase}?token={Uri.EscapeDataString(resetToken)}";

        var subject = "Відновлення пароля";
        var body =
            $"Щоб відновити пароль, перейдіть за посиланням:\n{resetLink}\n\n" +
            $"Або використайте цей токен у POST /api/v1/Auth/reset-password:\n{resetToken}\n\n" +
            "Посилання дійсне 1 годину. Якщо ви не запитували зміну пароля, проігноруйте цей лист.";

        if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
        {
            logger.LogWarning(
                "SMTP не налаштовано. Лист на {Email}. Посилання: {Link}. Токен: {Token}",
                toEmail,
                resetLink,
                resetToken);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = subject,
            Body = body
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = _settings.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_settings.SmtpUser))
        {
            client.Credentials = new NetworkCredential(
                _settings.SmtpUser,
                _settings.SmtpPassword);
        }

        await client.SendMailAsync(message);
    }
}
