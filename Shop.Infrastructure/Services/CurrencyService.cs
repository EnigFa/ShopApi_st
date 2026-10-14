using Newtonsoft.Json;
using Shop.Application.Interfaces.Services;

namespace Shop.Infrastructure.Services;

public class CurrencyService : ICurrencyService
{
    private const string ExchangeUrl =
        "https://7c49-37-52-79-159.ngrok-free.app/currency/nnn";

    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
            "ngrok-skip-browser-warning",
            "true");
    }

    public async Task<decimal> GetUsdRateAsync()
    {
        var response = await _httpClient.GetAsync(ExchangeUrl);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        
        dynamic? data = JsonConvert.DeserializeObject(content);

        if (data is null)
        {
            throw new InvalidOperationException("NBU response is empty.");
        }

        foreach (var item in data)
        {
            if ((string)item.cc == "USD")
            {
                return (decimal)item.rate;
            }
        }

        throw new InvalidOperationException("USD rate was not found in NBU response.");
    }
}
