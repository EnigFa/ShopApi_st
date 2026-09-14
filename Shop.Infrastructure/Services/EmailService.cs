using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Shop.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> emailOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public Task SendPasswordResetEmailAsync(
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

        return SendAsync(toEmail, subject, body);
    }

    public Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        IReadOnlyList<OrderEmailLineDTO> lines,
        decimal total)
    {
        var subject = $"Замовлення #{orderId} підтверджено";
        var body = BuildOrderBody(
            $"Ваше замовлення #{orderId} сформовано.",
            lines,
            total);

        return SendAsync(toEmail, subject, body);
    }

    public Task SendOrderWaitingAsync(
        string toEmail,
        int orderId,
        IReadOnlyList<OrderEmailLineDTO> lines,
        decimal total)
    {
        var subject = $"Замовлення #{orderId} очікує товар";
        var body = BuildOrderBody(
            $"Ваше замовлення #{orderId} прийнято, але частини товарів зараз немає на складі. Очікуйте надходження.",
            lines,
            total);

        return SendAsync(toEmail, subject, body);
    }

    private static string BuildOrderBody(
        string header,
        IReadOnlyList<OrderEmailLineDTO> lines,
        decimal total)
    {
        var builder = new StringBuilder();
        builder.AppendLine(header);
        builder.AppendLine();
        builder.AppendLine("Товари:");

        foreach (var line in lines)
        {
            builder.AppendLine(
                $"- {line.ProductName}: {line.Count} x {line.Price:0.00} = {line.LineTotal:0.00}");
        }

        builder.AppendLine();
        builder.AppendLine($"Загальна сума: {total:0.00}");
        return builder.ToString();
    }

    private async Task SendAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
        {
            logger.LogWarning(
                "SMTP не налаштовано. Лист на {Email}. Тема: {Subject}. Текст:\n{Body}",
                toEmail,
                subject,
                body);
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
