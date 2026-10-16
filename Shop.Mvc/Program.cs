using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Services;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using Shop.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});

builder.Services.AddAutoMapper(
    _ => { },
    typeof(CategoryProfile).Assembly,
    typeof(UserProfile).Assembly);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICachingService, MemoryCachingService>();
builder.Services.AddSingleton<IQueueService, NoOpQueueService>();
builder.Services.AddSingleton<IHashHelper, HashHelper>();
builder.Services.AddScoped<IFilePathProvider, FilePathProvider>();
builder.Services.AddScoped<IJWTService, JWTService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Categories/Index");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");

app.Run();
