using Application.Abstractions.Interactor;
using Application.Abstractions.Logging;
using Application.UseCase;
using static Contract.DomainEvent;

namespace Application.Interactors.Events
{
    public interface INewMotorcycleCreatedEvenInteractor : IInteractor<NewMotorcycleCreatedEvent, bool> { }

    public class NewMotorcycleCreatedEvenInteractor : INewMotorcycleCreatedEvenInteractor
    {
        private readonly IHandleNewMotorcycleCreatedUseCase _handleNewMotorcycleCreatedUseCase;
        private readonly ILoggerService<NewMotorcycleCreatedEvenInteractor> _logger;

        public NewMotorcycleCreatedEvenInteractor(
            IHandleNewMotorcycleCreatedUseCase handleNewMotorcycleCreatedUseCase,
            ILoggerService<NewMotorcycleCreatedEvenInteractor> logger)
        {
            _handleNewMotorcycleCreatedUseCase = handleNewMotorcycleCreatedUseCase ?? throw new ArgumentNullException(nameof(handleNewMotorcycleCreatedUseCase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> InteractAsync(NewMotorcycleCreatedEvent message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Recebido evento NewMotorcycleCreatedEvent: Id={Id}, Ano={Year}, Modelo={Model}, Placa={LicensePlate}",
                message.Id, message.Year, message.Model, message.LicensePlate);

            try
            {
                _logger.LogInformation("HandleNewMotorcycleCreatedUseCase - Iniciando a execução do caso de uso .");
                await _handleNewMotorcycleCreatedUseCase.ExecuteAsync(message, cancellationToken);
                _logger.LogInformation($"HandleNewMotorcycleCreatedUseCase - Evento NewMotorcycleCreatedEvent processado com sucesso: Id={message.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar o evento NewMotorcycleCreatedEvent: Id={message.Id}");
                throw;
            }
        }
    }
}
