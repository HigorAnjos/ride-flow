using MassTransit;
using System.Diagnostics.CodeAnalysis;

namespace Contract.Messages
{
    [ExcludeFromCodeCoverage]
    [ExcludeFromTopology]
    public abstract record Message : IMessage
    {
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
        public Guid? CorrelationId { get; init; } = Guid.NewGuid();
    }
}
