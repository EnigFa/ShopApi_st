using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shop.Api.Exceptions.HandlerExceptions;
using Shop.Api.Filters;
using Shop.Api.Helpers;
using Shop.Api.Interfaces;
using Shop.Api.Middlewares;
using Shop.Api.Services;
using Shop.Application;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Queries.Product;
using Shop.Application.Services;
using Shop.Application.Validators.Category;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Seeds;
using Shop.Infrastructure.Services;
using System.Text;

namespace Shop.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddScoped<ValidationFilter>();

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>(-3000);
        });
        //======================VALIDATORS=================
        builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

 
        var configuration = builder.Configuration;

        // ================= DATABASE =================

        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServerConnection"));
        });


        // ================= JWT SETTINGS =================

        var jwtSettings = configuration
            .GetSection("Jwt")
            .Get<JwtSettings>()
            ?? throw new Exception("JWT settings not configured.");

        builder.Services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));


        // ================= AUTHENTICATION (JWT + COOKIES + GOOGLE) =================

        var googleClientId = configuration["Authentication:Google:ClientId"];
        var googleClientSecret = configuration["Authentication:Google:ClientSecret"];

        var authentication = builder.Services
            .AddAuthentication(options =>
            {
                // Для стандартних API-запитів використовуємо JWT
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme); // Потрібно для зберігання стану OAuth

        if (!string.IsNullOrWhiteSpace(googleClientId) &&
            !string.IsNullOrWhiteSpace(googleClientSecret))
        {
            authentication.AddGoogle(options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
        }


        // ================= AUTHORIZATION =================

        builder.Services.AddAuthorization();


        // ================= AUTOMAPPER =================

        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly,
            typeof(UserProfile).Assembly
        );


        // ================= CORS =================

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });


        // ================= MEDIATR =================

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
        });


        // ================= CONTROLLERS =================

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();


        // ================= SWAGGER =================

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)
                    ] = []
                });
        });


        // ================= PROVIDERS =================

        builder.Services.AddScoped<IFilePathProvider, FilePathProvider>();


        // ================= APPLICATION SERVICES =================

        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddSingleton<IHashHelper, HashHelper>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
        builder.Services.AddScoped<IJWTService, JWTService>();


        builder.Services.AddHttpClient<ICurrencyService, CurrencyService>()
            .AddPolicyHandler(PollyHelper.GetRetryPolicy())
            .AddPolicyHandler(PollyHelper.GetCircuitBreakerPolicy());


        // ================= ADMIN SEEDER =================

        builder.Services.AddScoped<AdminSeeder>();


        // ================= CACHE =================

        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICachingService, MemoryCachingService>();
        builder.Services.AddSingleton<IQueueService, NoOpQueueService>();


        // ================= REPOSITORIES =================

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IDeliveryAddressRepository, DeliveryAddressRepository>();


        // ================= BUILD APP =================

        var app = builder.Build();


        // ================= DATABASE SEED =================

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
            await seeder.SeedAsync();
        }


        // ================= MIDDLEWARE =================

        app.UseExceptionHandler();

        app.UseCors("AllowAll");

        app.UseMiddleware<CancellationTokenHandleMiddleware>();

        // ================= SWAGGER =================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        // ================= HTTPS =================

        app.UseHttpsRedirection();


        // ================= AUTH =================

        app.UseAuthentication();
        app.UseAuthorization();


        // ================= STATIC FILES =================

        app.UseStaticFiles();


        // ================= CONTROLLERS =================

        app.MapControllers();


        // ================= RUN =================

        app.Run();
    }
}