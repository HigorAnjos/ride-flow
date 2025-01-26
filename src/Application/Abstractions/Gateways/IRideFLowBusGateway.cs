using Contract.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Gateways
{
    public interface IRideFLowBusGateway
    {
        Task PublishAsync(IMessage message, CancellationToken cancellationToken);

        Task PublishBatchAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken);
    }
}
