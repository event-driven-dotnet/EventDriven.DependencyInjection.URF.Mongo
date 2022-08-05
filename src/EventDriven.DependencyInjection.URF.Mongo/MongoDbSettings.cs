namespace EventDriven.DependencyInjection.URF.Mongo;

/// <inheritdoc />
public abstract class MongoDbSettings : IMongoDbSettings
{
    /// <inheritdoc />
    public string ConnectionString { get; set; } = null!;

    /// <inheritdoc />
    public string DatabaseName { get; set; } = null!;

    /// <inheritdoc />
    public string CollectionName { get; set; } = null!;
}