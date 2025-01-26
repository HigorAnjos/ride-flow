using Application.Abstractions.Gateways;
using Application.Abstractions.Logging;
using Application.Extensions;
using Infrastructure.MessageBus.Bus;
using Infrastructure.MessageBus.Gateways;
using Infrastructure.MessageBus.Options;
using Infrastructure.RideFlowBus.Consumers.Events;
using Infrastructure.RideFlowBus.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Contract.DomainEvent;

namespace Infrastructure.MessageBus.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureRideFlow(this IServiceCollection services, IConfiguration configuration)
            => services.AddGateways()
                       .ConfigureRideFlowMessageBus(configuration)
                       .AddLoggingServices();

        private static IServiceCollection AddGateways(this IServiceCollection services)
        {
            services.AddScoped<IRideFLowBusGateway, RideFlowBusGateway>();
            return services;
        }

        private static IServiceCollection ConfigureRideFlowMessageBus(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddMessageBus(configuration)
                .ConfigureOptions<RideFlowBusOptions>(configuration.GetSection(nameof(RideFlowBusOptions)))
                .ConfigureBus<IRideFlowBus, RideFlowBusOptions>(
                (bus, context) =>
                {
                    // Events
                    bus.ConfigureRideFlowEndpoint<NewMotorcycleCreatedEventConsumer, NewMotorcycleCreatedEvent>(context);

                });

        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
            return services;
        }
    }
}
