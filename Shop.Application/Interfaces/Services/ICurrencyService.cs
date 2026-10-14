namespace Shop.Application.Interfaces.Services;

public interface ICurrencyService
{
    Task<decimal> GetUsdRateAsync();
}
