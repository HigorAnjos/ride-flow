using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;

namespace Application.UseCase
{
    public interface IGetMotorcycleByIdUseCase
    {
        Task<Motorcycle> ExecuteAsync(string motorcycleId, CancellationToken cancellation);
    }

    public class GetMotorcycleByIdUseCase : BaseUseCase<GetMotorcycleByIdUseCase>, IGetMotorcycleByIdUseCase
    {
        private readonly IMotorcycleRepository _repository;

        protected override string ActionIdentification { get; } = "GetMotorcycleByIdUseCase";


        public GetMotorcycleByIdUseCase(IMotorcycleRepository repository, ILoggerService<GetMotorcycleByIdUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Motorcycle> ExecuteAsync(string motorcycleId, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando a busca pela motocicleta. ID: {motorcycleId}");

            if (string.IsNullOrWhiteSpace(motorcycleId))
            {
                LogWarning("O ID fornecido para a busca está vazio ou nulo.");
                return null;
            }

            try
            {
                LogInformation($"Procurando a motocicleta no repositório. ID: {motorcycleId}");
                var motorcycle = await _repository.GetByIdAsync(motorcycleId, cancellationToken);

                if (motorcycle == null)
                {
                    LogWarning($"Nenhuma motocicleta encontrada para o ID: {motorcycleId}");
                }
                else
                {
                    LogInformation($"Motocicleta encontrada. ID: {motorcycle.Id}, Modelo: {motorcycle.Model}, Placa: {motorcycle.LicensePlate}");
                }

                return motorcycle;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao buscar a motocicleta. ID: {motorcycleId}");
                throw;
            }
        }
    }
}
