using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace Contract.Messages
{
    [ExcludeFromTopology]
    public interface IMessage
    {
        DateTimeOffset Timestamp { get; }
        Guid? CorrelationId { get; }
    }
}
