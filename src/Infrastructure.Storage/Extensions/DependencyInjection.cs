using Application.Abstractions.Repositories;
using Application.Enumerations;
using Infrastructure.Storage.Configurations;
using Infrastructure.Storage.Options;
using Infrastructure.Storage.Repositories;
using Infrastructure.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Storage.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureStorage(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddRepositories()
            .AddDataOptions(configuration)
            .AddMongoDb(configuration);

        private static IServiceCollection AddRepositories(this IServiceCollection services)
            => services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>()
                       .AddScoped<IMotorcycleRepository, MotorcycleRepository>()
                       .AddScoped<IRentalRepository, RentalRepository>()
                       .AddScoped<IMotorcycleEventRepository, MotorcycleEventRepository>()
                       .AddScoped<IImageStorageService, ImageStorageService>();

        private static IServiceCollection AddDataOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RideFlowSqlServerOptions>(options =>
            {
                configuration.GetSection("ConnectionStrings").Bind(options);
            });

            return services
                .AddMemoryCache()
                .AddTransient<IConnectionFactory, ConnectionFactory>()
                .AddScoped<IScriptLoader, ScriptLoader>()
                .AddScoped<IUnitOfWork>(s =>
                {
                    var dbProvider = s.GetRequiredService<IConnectionFactory>();
                    return new UnitOfWork(dbProvider.CreateConnection(Databases.RIDE_FLOW_POSTGRES));
                });
        }

        private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar mapeamentos do MongoDB
            MongoDbMappings.RegisterMappings();

            var mongoConnectionString = configuration.GetValue<string>("ConnectionStrings:RideFlowMongo");

            // Registra o MongoClient como singleton
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));

            // Registra o banco de dados do MongoDB como singleton
            services.AddSingleton(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase("RideFlowDatabase");
            });

            return services;
        }
    }
}
