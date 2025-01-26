using MassTransit;

namespace Contract.Messages
{
    [ExcludeFromTopology]
    public interface ICommand : IMessage { }
}
