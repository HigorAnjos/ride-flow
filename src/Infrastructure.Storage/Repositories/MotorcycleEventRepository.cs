using Application.Abstractions.Repositories;
using Contract.Messages;
using MongoDB.Driver;
using static Contract.DomainEvent;

namespace Infrastructure.Storage.Repositories
{
    public class MotorcycleEventRepository : IMotorcycleEventRepository
    {
        private readonly IMongoCollection<NewMotorcycleCreatedEvent> _eventsCollection;

        public MotorcycleEventRepository(IMongoDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database), "O banco de dados não pode ser nulo.");

            _eventsCollection = database.GetCollection<NewMotorcycleCreatedEvent>("MotorcycleEvents")
                ?? throw new InvalidOperationException("Não foi possível obter a coleção MotorcycleEvents.");
        }

        public async Task SaveEventAsync(NewMotorcycleCreatedEvent motorcycleEvent, CancellationToken cancellationToken)
        {
            ValidateEvent(motorcycleEvent);
            await _eventsCollection.InsertOneAsync(motorcycleEvent, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<NewMotorcycleCreatedEvent>> GetEventsByYearAsync(int year, CancellationToken cancellationToken)
        {
            var filter = Builders<NewMotorcycleCreatedEvent>.Filter.Eq("Year", year);
            return await _eventsCollection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task DeleteEventByIdAsync(string eventId, CancellationToken cancellationToken)
        {
            ValidateId(eventId);
            var filter = Builders<NewMotorcycleCreatedEvent>.Filter.Eq("_id", eventId);
            var result = await _eventsCollection.DeleteOneAsync(filter, cancellationToken);

            if (result.DeletedCount == 0)
                throw new InvalidOperationException($"Nenhum evento encontrado com o ID: {eventId}");
        }

        public async Task<IEnumerable<NewMotorcycleCreatedEvent>> GetAllEventsAsync(CancellationToken cancellationToken)
        {
            return await _eventsCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        private static void ValidateEvent(NewMotorcycleCreatedEvent motorcycleEvent)
        {
            if (motorcycleEvent == null)
                throw new ArgumentNullException(nameof(motorcycleEvent), "O evento não pode ser nulo.");
        }

        private static void ValidateId(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentException("O ID do evento não pode ser vazio.", nameof(eventId));
        }
    }
}
