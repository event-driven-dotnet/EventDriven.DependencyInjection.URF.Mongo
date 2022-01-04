namespace EventDriven.DependencyInjection.URF.Mongo;

/// <summary>
/// MongoDB settings.
/// </summary>
public interface IMongoDbSettings
{
    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// Database name.
    /// </summary>
    public string DatabaseName { get; set; }
    /// <summary>
    /// Collection name.
    /// </summary>
    public string CollectionName { get; set; }
}