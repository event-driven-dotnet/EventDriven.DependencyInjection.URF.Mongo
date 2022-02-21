using EventDriven.DependencyInjection.URF.Mongo;

namespace MongoDbSettingsSample.Configuration;

public class MyOtherMongoSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}