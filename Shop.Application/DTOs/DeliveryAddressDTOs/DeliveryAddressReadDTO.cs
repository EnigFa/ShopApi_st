namespace Shop.Application.DTOs.DeliveryAddressDTOs;

public class DeliveryAddressReadDTO
{
    public int Id { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string? Building { get; set; }

    public string? Apartment { get; set; }

    public string? PostalCode { get; set; }

    public bool IsDefault { get; set; }
}
