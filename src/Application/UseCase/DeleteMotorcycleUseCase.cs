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
        private readonly IMotorcycleRepository _repository;

        protected override string ActionIdentification { get; } = "DeleteMotorcycleUseCase";


        public DeleteMotorcycleUseCase(IMotorcycleRepository repository, ILoggerService<DeleteMotorcycleUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
                LogInformation($"Excluindo a motocicleta do repositório. ID: {motorcycleId}");
                await _repository.DeleteAsync(motorcycleId);
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
