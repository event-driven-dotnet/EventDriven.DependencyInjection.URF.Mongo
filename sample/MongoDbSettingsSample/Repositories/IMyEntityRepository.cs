namespace MongoDbSettingsSample.Repositories;

public interface IEntityRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAsync();
    Task<TEntity> GetAsync(Guid id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task<TEntity?> UpdateAsync(TEntity entity);
    Task<int> RemoveAsync(Guid id);

}