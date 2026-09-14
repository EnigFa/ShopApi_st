using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);

    Task SendOrderConfirmationAsync(
        string toEmail,
        int orderId,
        IReadOnlyList<OrderEmailLineDTO> lines,
        decimal total);

    Task SendOrderWaitingAsync(
        string toEmail,
        int orderId,
        IReadOnlyList<OrderEmailLineDTO> lines,
        decimal total);
}
