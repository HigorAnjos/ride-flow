using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using static Contract.DomainEvent;

namespace Application.UseCase
{
    public interface IHandleNewMotorcycleCreatedUseCase
    {
        Task ExecuteAsync(NewMotorcycleCreatedEvent eventMessage, CancellationToken cancellationToken);
    }

    public class HandleNewMotorcycleCreatedUseCase : BaseUseCase<HandleNewMotorcycleCreatedUseCase>, IHandleNewMotorcycleCreatedUseCase
    {
        private readonly IMotorcycleEventRepository _repository;

        protected override string ActionIdentification { get; } = "HandleNewMotorcycleCreatedUseCase";

        public HandleNewMotorcycleCreatedUseCase(IMotorcycleEventRepository repository, ILoggerService<HandleNewMotorcycleCreatedUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task ExecuteAsync(NewMotorcycleCreatedEvent eventMessage, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando o processamento do evento de motocicleta criada. Evento: {eventMessage?.Id}");

            try
            {
                if (eventMessage == null)
                {
                    LogWarning("O evento fornecido está nulo.");
                    throw new ArgumentNullException(nameof(eventMessage), "O evento não pode ser nulo.");
                }

                if (eventMessage.Year != 2024)
                {
                    LogInformation($"Evento ignorado, pois o ano da motocicleta ({eventMessage.Year}) não é 2024.");
                    return;
                }

                await _repository.SaveEventAsync(eventMessage, cancellationToken);
                LogInformation($"Evento salvo com sucesso no repositório. Evento: {eventMessage.Id}");
            }
            catch (ArgumentNullException ex)
            {
                LogError(ex, $"Erro de argumento nulo ao processar o evento. Mensagem: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro inesperado ao processar o evento. Evento: {eventMessage?.Id}");
                throw; 
            }
        }
    }
}
