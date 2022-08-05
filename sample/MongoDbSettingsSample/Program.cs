using EventDriven.DependencyInjection.URF.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDbSettingsSample.Configuration;
using MongoDbSettingsSample.Models;
using MongoDbSettingsSample.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

const bool cleanup = true;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        services.AddSingleton<IEntityRepository<MyEntity>, EntityRepository<MyEntity>>();
        services.AddSingleton<IEntityRepository<MyOtherEntity>, EntityRepository<MyOtherEntity>>();
        services.AddMongoDbSettings<MyMongoSettings, MyEntity>(config);
        services.AddMongoDbSettings<MyOtherMongoSettings, MyOtherEntity>(config);
        services.AddMongoDbTransactionSettings<MyMongoSettings, DocumentUnitOfWork>(config);
    })
    .Build();

await UseRepository(host, cleanup,
    new MyEntity { Id = Guid.NewGuid(), StringValue = "Hello 1", IntValue = 10 },
    new MyEntity { Id = Guid.NewGuid(), StringValue = "Hello 2", IntValue = 20 },
    new MyEntity { Id = Guid.NewGuid(), StringValue = "Hello 3", IntValue = 30 });

await UseRepository(host, cleanup,
    new MyOtherEntity { Id = Guid.NewGuid(), OtherStringValue = "Hello 4", OtherIntValue = 40 },
    new MyOtherEntity { Id = Guid.NewGuid(), OtherStringValue = "Hello 5", OtherIntValue = 50 },
    new MyOtherEntity { Id = Guid.NewGuid(), OtherStringValue = "Hello 6", OtherIntValue = 60 });

await UseUnitOfWork(host);

async Task UseRepository<TEntity>(IHost host1, bool cleanup1, params TEntity[] entities1)
    where TEntity : EntityBase, new()
{
    var repo = host1.Services.GetRequiredService<IEntityRepository<TEntity>>();
    var entity1 = await repo.AddAsync(entities1[0]);
    var entity2 = await repo.AddAsync(entities1[1]);
    var entity3 = await repo.AddAsync(entities1[2]);

    var entities = await repo.GetAsync();
    foreach (var entity in entities)
        Console.WriteLine($"{entity.Id}");

    if (cleanup1)
    {
        await repo.RemoveAsync(entity1!.Id);
        await repo.RemoveAsync(entity2!.Id);
        await repo.RemoveAsync(entity3!.Id);
    }
}

async Task UseUnitOfWork(IHost host1)
{
    var unitOfWork = host1.Services.GetRequiredService<IDocumentUnitOfWork>();
    try
    {
        await unitOfWork.StartTransactionAsync();
        await unitOfWork.CommitAsync();
    }
    catch (NotSupportedException e)
    {
        Console.WriteLine(e.Message);
    }
}
