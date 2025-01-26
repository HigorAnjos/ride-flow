using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using System.Threading;

namespace Application.UseCase
{
    public interface IDeleteMotorcycleUseCase
    {
        Task ExecuteAsync(string motorcycleId, CancellationToken cancellationToken);
    }

    public class DeleteMotorcycleUseCase : BaseUseCase<DeleteMotorcycleUseCase>, IDeleteMotorcycleUseCase
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IRentalRepository _rentalRepository;

        protected override string ActionIdentification { get; } = "DeleteMotorcycleUseCase";

        public DeleteMotorcycleUseCase(
            IMotorcycleRepository motorcycleRepository,
            IRentalRepository rentalRepository,
            ILoggerService<DeleteMotorcycleUseCase> logger)
            : base(logger)
        {
            _motorcycleRepository = motorcycleRepository ?? throw new ArgumentNullException(nameof(motorcycleRepository));
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        }

        public async Task ExecuteAsync(string motorcycleId, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando exclusão da motocicleta. ID: {motorcycleId}");

            if (string.IsNullOrWhiteSpace(motorcycleId))
            {
                LogWarning("O ID da moto é obrigatório.");
                throw new ArgumentException("O ID da moto é obrigatório.", nameof(motorcycleId));
            }

            try
            {
                // Verificar se existem registros de locações associados à motocicleta
                var hasRentalRecords = await _rentalRepository.HasRentalsAsync(motorcycleId, cancellationToken);
                if (hasRentalRecords)
                {
                    LogWarning($"A motocicleta com ID: {motorcycleId} possui registros de locações e não pode ser removida.");
                    throw new InvalidOperationException($"A motocicleta com ID: {motorcycleId} possui registros de locações e não pode ser removida.");
                }

                // Excluir a motocicleta
                LogInformation($"Excluindo a motocicleta do repositório. ID: {motorcycleId}");
                await _motorcycleRepository.DeleteAsync(motorcycleId, cancellationToken);
                LogInformation($"Motocicleta excluída com sucesso. ID: {motorcycleId}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao excluir a motocicleta. ID: {motorcycleId}");
                throw;
            }
        }
    }
}
