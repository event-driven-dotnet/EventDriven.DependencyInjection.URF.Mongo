namespace MongoDbSettingsSample.Models;

public class EntityBase
{
    public Guid Id { get; set; }
    public string ETag { get; set; } = null!;
}