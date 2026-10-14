using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Application.Interfaces.Helpers;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using StackExchange.Redis;

namespace Shop.Infrastructure.Seeds;

public class AdminSeeder
{
    private readonly ShopDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IHashHelper _hashHelper;

    public AdminSeeder(
        ShopDbContext db,
        IConfiguration configuration,
        IHashHelper hashHelper)
    {
        _db = db;
        _configuration = configuration;
        _hashHelper = hashHelper;
    }

    public async Task SeedAsync()
    {
        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];

        // Якщо секрети не задані — нічого не робимо
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        // Перевіряємо, чи вже існує такий користувач
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingUser is not null)
        {
            return;
        }

        // Створюємо першого адміністратора
        var admin = new User
        {
            Email = email,
            PasswordHash = _hashHelper.Hash(password),
            Role = UserRole.Admin
        };

        _db.Users.Add(admin);

        await _db.SaveChangesAsync();
    }
}