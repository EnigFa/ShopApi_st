using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Documents;

namespace Shop.Infrastructure.Services;

public class MongoProductFeedbackService(
    IMongoClient mongoClient,
    IOptions<MongoDbSettings> options,
    IProductRepository productRepository,
    ILogger<MongoProductFeedbackService> logger) : IProductFeedbackService
{
    private readonly IMongoCollection<ProductFeedbackDocument> _collection =
        mongoClient
            .GetDatabase(options.Value.DatabaseName)
            .GetCollection<ProductFeedbackDocument>(options.Value.ProductFeedbackCollection);

    public async Task<ProductFeedbackReadDTO> AddAsync(
        int productId,
        ProductFeedbackCreateDTO dto,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        if (product is null)
        {
            throw new KeyNotFoundException($"Товар {productId} не знайдено");
        }

        if (dto.Kind == ProductFeedbackKind.Review && dto.Rating is null)
        {
            throw new ArgumentException("Для відгуку потрібна оцінка від 1 до 5");
        }

        var document = new ProductFeedbackDocument
        {
            ProductId = productId,
            Kind = dto.Kind,
            AuthorName = string.IsNullOrWhiteSpace(dto.AuthorName) ? null : dto.AuthorName.Trim(),
            AuthorEmail = string.IsNullOrWhiteSpace(dto.AuthorEmail) ? null : dto.AuthorEmail.Trim(),
            Rating = dto.Kind == ProductFeedbackKind.Review ? dto.Rating : null,
            Text = dto.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);

        logger.LogInformation(
            "Додано {Kind} для товару {ProductId} ({FeedbackId})",
            document.Kind,
            productId,
            document.Id);

        return new ProductFeedbackReadDTO
        {
            Id = document.Id,
            ProductId = document.ProductId,
            Kind = document.Kind,
            AuthorName = document.AuthorName,
            AuthorEmail = document.AuthorEmail,
            Rating = document.Rating,
            Text = document.Text,
            CreatedAt = document.CreatedAt
        };
    }
}
