using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;

namespace EventDriven.DependencyInjection.URF.Mongo
{
    /// <summary>
    /// Helper methods for configuring services with dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register MongoDB collection with dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="config">The application's <see cref="IConfiguration"/>.</param>
        /// <param name="lifetime">Service lifetime.</param>
        /// <typeparam name="TAppSettings">Strongly typed app settings class.</typeparam>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongoDbSettings<TAppSettings, TEntity>(
            this IServiceCollection services, IConfiguration config,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TAppSettings : class, IMongoDbSettings
            where TEntity : class =>
            services.AddMongoDbSettings<TAppSettings, TEntity, TEntity>(config, lifetime);

        /// <summary>
        /// Register MongoDB collection with dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="config">The application's <see cref="IConfiguration"/>.</param>
        /// <param name="lifetime">Service lifetime.</param>
        /// <typeparam name="TAppSettings">Strongly typed app settings class.</typeparam>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TDocumentEntity">Document entity type.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongoDbSettings<TAppSettings, TEntity, TDocumentEntity>(
            this IServiceCollection services, IConfiguration config,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TAppSettings : class, IMongoDbSettings
            where TEntity : class
            where TDocumentEntity : class
        {
            IMongoCollection<TEntity> GetMongoCollection(IServiceProvider sp)
            {
                var settings = sp.GetRequiredService<TAppSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<TEntity>(settings.CollectionName);
            }
            services.AddAppSettings<TAppSettings>(config, lifetime);
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(GetMongoCollection);
                    services.AddTransient<IDocumentRepository<TDocumentEntity>, DocumentRepository<TDocumentEntity>>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(GetMongoCollection);
                    services.AddScoped<IDocumentRepository<TDocumentEntity>, DocumentRepository<TDocumentEntity>>();
                    break;
                default:
                    services.AddSingleton(GetMongoCollection);
                    services.AddSingleton<IDocumentRepository<TDocumentEntity>, DocumentRepository<TDocumentEntity>>();
                    break;
            }
            return services;
        }

        /// <summary>
        /// Register MongoDB database with dependency injection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="config">The application's <see cref="IConfiguration"/>.</param>
        /// <param name="lifetime">Service lifetime.</param>
        /// <typeparam name="TAppSettings">Strongly typed app settings class.</typeparam>
        /// <typeparam name="TDocumentUnitOfWork">Document unit of work.</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddMongoDbTransactionSettings<TAppSettings, TDocumentUnitOfWork>(
            this IServiceCollection services, IConfiguration config,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TAppSettings : class, IMongoDbSettings
            where TDocumentUnitOfWork : class, IDocumentUnitOfWork
        {
            IMongoDatabase GetMongoDatabase(IServiceProvider sp)
            {
                var settings = sp.GetRequiredService<TAppSettings>();
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            }
            services.AddAppSettings<TAppSettings>(config, lifetime);
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(GetMongoDatabase);
                    services.AddTransient<IDocumentUnitOfWork, TDocumentUnitOfWork>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(GetMongoDatabase);
                    services.AddScoped<IDocumentUnitOfWork, TDocumentUnitOfWork>();
                    break;
                default:
                    services.AddSingleton(GetMongoDatabase);
                    services.AddSingleton<IDocumentUnitOfWork, TDocumentUnitOfWork>();
                    break;
            }
            return services;
        }
    }
}