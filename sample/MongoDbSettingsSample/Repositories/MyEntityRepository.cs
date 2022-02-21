using MongoDB.Driver;
using MongoDbSettingsSample.Helpers;
using MongoDbSettingsSample.Models;
using URF.Core.Mongo;

namespace MongoDbSettingsSample.Repositories;

public class EntityRepository<TEntity> : DocumentRepository<TEntity>, IEntityRepository<TEntity>
    where TEntity : EntityBase
{
    public EntityRepository(IMongoCollection<TEntity> collection) : base(collection)
    {
    }

    public async Task<IEnumerable<TEntity>> GetAsync() => 
        await FindManyAsync();

    public async Task<TEntity> GetAsync(Guid id) => 
        await FindOneAsync(e => e.Id == id);

    public async Task<TEntity?> AddAsync(TEntity entity)
    {
        var existing = await FindOneAsync(e => e.Id == entity.Id);
        if (existing != null) return null;
        entity.ETag = Guid.NewGuid().ToString();
        return await InsertOneAsync(entity);
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        var existing = await GetAsync(entity.Id);
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> RemoveAsync(Guid id) => 
        await DeleteOneAsync(e => e.Id == id);
}