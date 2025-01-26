using Infrastructure.MessageBus.Options;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contract.Extensions;

namespace Infrastructure.MessageBus.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MassTransitHostOptions>(options =>
            {
                configuration.GetSection(nameof(MassTransitHostOptions)).Bind(options);
            });

            return services;
        }

        public static IServiceCollection ConfigureBus<TBus, TOptions>(this IServiceCollection services,
            Action<IRabbitMqBusFactoryConfigurator, IRegistrationContext>? configureConsumers = null)
            where TBus : class, IBus
            where TOptions : BusOptions
            => services
                .AddMassTransit<TBus>(cfg =>
                {
                    var options = services
                        .BuildServiceProvider()
                        .GetRequiredService<IOptionsMonitor<TOptions>>()
                        .CurrentValue;

                    cfg.SetKebabCaseEndpointNameFormatter();
                    cfg.AddConsumers(typeof(TBus).Assembly);
                    cfg.UsingRabbitMq((context, bus) =>
                    {
                        bus.Host(options.ConnectionString, "Ride.Flow", hostConfig =>
                        {
                            if (options.Cluster is not null && options.Cluster.Any())
                                hostConfig.UseCluster(clusterConfig => options.Cluster.ForEach(clusterConfig.Node));
                        });

                        bus.UseMessageRetry(retry
                           => retry.Incremental(
                               retryLimit: options.RetryLimit,
                               initialInterval: options.InitialInterval,
                               intervalIncrement: options.IntervalIncrement));

                        bus.UseNewtonsoftJsonSerializer();

                        bus.ConfigurePublish(pipe => pipe.AddPipeSpecification(new DelegatePipeSpecification<PublishContext>(p => p.CorrelationId = p.InitiatorId.Coalesce(p.CorrelationId, Guid.NewGuid()))));

                        configureConsumers?.Invoke(bus, context);

                        bus.ConfigureEndpoints(context);
                    });
                });
    }
}
