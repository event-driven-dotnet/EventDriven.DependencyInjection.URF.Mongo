using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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
        /// <typeparam name="TAppSettings">Strongly typed app settings class.</typeparam>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMongoDbSettings<TAppSettings, TEntity>(
            this IServiceCollection services, IConfiguration config)
            where TAppSettings : class, IMongoDbSettings
        {
            services.AddAppSettings<TAppSettings>(config);
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<TAppSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<TEntity>(settings.CollectionName);
            });
            return services;
        }
    }
}