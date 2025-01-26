using static Contract.DomainEvent;

namespace Application.Abstractions.Repositories
{
    public interface IMotorcycleEventRepository
    {
        /// <summary>
        /// Salva um evento de moto no MongoDB.
        /// </summary>
        /// <param name="motorcycleEvent">Objeto contendo os dados do evento de moto.</param>
        Task SaveEventAsync(NewMotorcycleCreatedEvent motorcycleEvent, CancellationToken cancellationToken);

        /// <summary>
        /// Obtém todos os eventos de motos do ano especificado.
        /// </summary>
        /// <param name="year">Ano das motos para filtrar os eventos.</param>
        Task<IEnumerable<NewMotorcycleCreatedEvent>> GetEventsByYearAsync(int year, CancellationToken cancellationToken);

        /// <summary>
        /// Exclui um evento de moto específico pelo identificador.
        /// </summary>
        /// <param name="eventId">ID único do evento.</param>
        Task DeleteEventByIdAsync(string eventId, CancellationToken cancellationToken);

        /// <summary>
        /// Obtém todos os eventos armazenados.
        /// </summary>
        Task<IEnumerable<NewMotorcycleCreatedEvent>> GetAllEventsAsync(CancellationToken cancellationToken);
    }
}
