using Contract.Messages;

namespace Contract
{
    public class DomainEvent
    {
        public record NewMotorcycleCreatedEvent(string Id, int Year, string Model, string LicensePlate) : Message, IEvent;
    }
}
