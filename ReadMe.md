# EventDriven.DependencyInjection.Mongo

Helper methods for configuring services for MongoDB with dependency injection.

### Setup

- Package references.
  - EventDriven.DependencyInjection.URF.Mongo

- appsettings.json file.

```json
{
  "MyMongoSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "SagaConfigDb",
    "CollectionName": "MyEntities"
  }
}
```

- Strongly typed app settings class.

```csharp
public class MyMongoSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}
```

- Entity class.

```csharp
public class MyEntity
{
    public Guid Id { get; set; }
    public string StringValue { get; set; } = null!;
    public int MyIntValue { get; set; }
}
```

#### Usage

- In a Web API project.

```csharp
builder.Services.AddMongoDbSettings<MyMongoSettings, MyEntity>(builder.Configuration);
```

- In a console project.

```csharp
var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        services.AddMongoDbSettings<MyMongoSettings, MyEntity>(config);
    })
    .Build();
```