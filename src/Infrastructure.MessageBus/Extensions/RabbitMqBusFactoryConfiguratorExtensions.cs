using MassTransit;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Infrastructure.MessageBus.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class RabbitMqBusFactoryConfiguratorExtensions
    {
        public static void ConfigureRideFlowEndpoint<TConsumer, TMessage>(
             this IRabbitMqBusFactoryConfigurator bus,
             IRegistrationContext context,
             string? exchangeName = null)
             where TConsumer : class, IConsumer<TMessage>
             where TMessage : class
             => bus.ReceiveEndpoint(
                 queueName: GetDescription<TConsumer, TMessage>(),
                 configureEndpoint: endpoint =>
                 {
                     endpoint.ConfigureConsumeTopology = false;
                     endpoint.ConfigureConsumer<TConsumer>(context);

                     if (!string.IsNullOrWhiteSpace(exchangeName))
                         endpoint.Bind(exchangeName);
                     else
                         endpoint.Bind<TMessage>();
                 });


        private static string GetDescription<TConsumer, TMessage>() =>
            $"ride.flow.{typeof(TConsumer).ToKebabCaseString()}.{typeof(TMessage).ToKebabCaseString()}";

    }
}
