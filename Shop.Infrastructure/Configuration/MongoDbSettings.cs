namespace Shop.Infrastructure.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    public string DatabaseName { get; set; } = "ShopDb";

    public string ProductFeedbackCollection { get; set; } = "product_feedback";
}
