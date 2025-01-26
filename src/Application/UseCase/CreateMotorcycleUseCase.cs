using Application.Abstractions.Gateways;
using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;
using static Contract.DomainEvent;

namespace Application.UseCase
{
    public interface ICreateMotorcycleUseCase
    {
        Task<Motorcycle> ExecuteAsync(string id, int year, string model, string licensePlate, CancellationToken cancellationToken);
    }

    public class CreateMotorcycleUseCase : BaseUseCase<CreateMotorcycleUseCase>, ICreateMotorcycleUseCase
    {
        private readonly IMotorcycleRepository _repository;
        private readonly IRideFLowBusGateway _publisher;

        protected override string ActionIdentification { get; } = "CreateMotorcycleUseCase";

        public CreateMotorcycleUseCase(
            IMotorcycleRepository repository,
            IRideFLowBusGateway publisher,
            ILoggerService<CreateMotorcycleUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<Motorcycle> ExecuteAsync(string id, int year, string model, string licensePlate, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando a criação de uma nova motocicleta. ID: {id}, Ano: {year}, Modelo: {model}, Placa: {licensePlate}");

            // Se não for passado o ID, gerar um novo
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
                LogInformation($"Nenhum ID fornecido. Gerado um novo ID: {id}");
            }

            var motorcycle = new Motorcycle
            {
                Id = id,
                Year = year,
                Model = model,
                LicensePlate = licensePlate
            };

            // Validar os dados da motocicleta
            if (!motorcycle.IsValid())
            {
                LogWarning($"Os dados da motocicleta são inválidos. ID: {id}, Ano: {year}, Modelo: {model}, Placa: {licensePlate}");
                throw new InvalidDataException("Os dados da motocicleta são inválidos.");
            }

            // Verifica se a placa já existe
            var existingMotorcycle = await _repository.GetByLicensePlateAsync(motorcycle.LicensePlate);
            if (existingMotorcycle != null)
            {
                LogWarning($"Já existe uma motocicleta com a placa fornecida: {licensePlate}");
                throw new InvalidDataException("A placa fornecida já está registrada.");
            }

            // Salva a motocicleta no repositório
            try
            {
                LogInformation($"Salvando a motocicleta no repositório. ID: {id}, Placa: {licensePlate}");
                await _repository.AddAsync(motorcycle);
                LogInformation($"Motocicleta salva com sucesso. ID: {id}, Placa: {licensePlate}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao salvar a motocicleta. ID: {id}, Placa: {licensePlate}");
                throw;
            }

            // Publica o evento de criação
            try
            {
                LogInformation($"Publicando o evento NewMotorcycleCreatedEvent. ID: {id}");
                await _publisher.PublishAsync(new NewMotorcycleCreatedEvent(id, year, model, licensePlate), cancellationToken);
                LogInformation($"Evento publicado com sucesso. ID: {id}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao publicar o evento NewMotorcycleCreatedEvent. ID: {id}");
                throw;
            }

            // Retorna a motocicleta criada
            return motorcycle;
        }
    }
}
