namespace Shop.Infrastructure.Configuration;

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;

    public string SmtpUser { get; set; } = string.Empty;

    public string SmtpPassword { get; set; } = string.Empty;

    public bool EnableSsl { get; set; } = true;

    public string From { get; set; } = "noreply@shop.local";

    public string PasswordResetLinkBase { get; set; } = string.Empty;
}
