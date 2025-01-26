using Application.Abstractions.Gateways;
using Contract.Messages;
using Infrastructure.MessageBus.Bus;
using MassTransit;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MessageBus.Gateways
{
    public class RideFlowBusGateway : IRideFLowBusGateway
    {
        private readonly IBusInstance<IRideFlowBus> _busInstance;

        public RideFlowBusGateway(IBusInstance<IRideFlowBus> busInstance)
        {
            _busInstance = busInstance;
        }

        public Task PublishAsync(IMessage message, CancellationToken cancellationToken)
            => _busInstance.Bus.Publish(message, message.GetType(), cancellationToken);

        public Task PublishBatchAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken)
            => Task.WhenAll(messages.Select(message=> _busInstance.Bus.Publish(message, message.GetType(), cancellationToken)));
    }
}
