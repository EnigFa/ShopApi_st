using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shop.Domain.Enums;

namespace Shop.Infrastructure.Documents;

public class ProductFeedbackDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("productId")]
    public int ProductId { get; set; }

    [BsonElement("kind")]
    [BsonRepresentation(BsonType.String)]
    public ProductFeedbackKind Kind { get; set; }

    [BsonElement("authorName")]
    [BsonIgnoreIfNull]
    public string? AuthorName { get; set; }

    [BsonElement("authorEmail")]
    [BsonIgnoreIfNull]
    public string? AuthorEmail { get; set; }

    [BsonElement("rating")]
    [BsonIgnoreIfNull]
    public int? Rating { get; set; }

    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}
