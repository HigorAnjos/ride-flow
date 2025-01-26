using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interactors.Events;
using MassTransit;
using static Contract.DomainEvent;

namespace Infrastructure.RideFlowBus.Consumers.Events
{
    [ExcludeFromCodeCoverage]
    public class NewMotorcycleCreatedEventConsumer : IConsumer<NewMotorcycleCreatedEvent>
    {
        private readonly INewMotorcycleCreatedEvenInteractor _interactor;

        public NewMotorcycleCreatedEventConsumer(INewMotorcycleCreatedEvenInteractor interactor)
        {
            _interactor = interactor;
        }

        public Task Consume(ConsumeContext<NewMotorcycleCreatedEvent> context)
            => _interactor.InteractAsync(context.Message, context.CancellationToken);
    }
}
