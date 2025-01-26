using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;

namespace Application.UseCase
{
    public interface IGetMotorcyclesUseCase
    {
        Task<IEnumerable<Motorcycle>> ExecuteAsync(CancellationToken cancellationToken, string? licensePlate = null);
    }

    public class GetMotorcyclesUseCase : BaseUseCase<GetMotorcyclesUseCase>, IGetMotorcyclesUseCase
    {
        private readonly IMotorcycleRepository _repository;

        protected override string ActionIdentification { get; } = "GetMotorcyclesUseCase";

        public GetMotorcyclesUseCase(IMotorcycleRepository repository, ILoggerService<GetMotorcyclesUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<Motorcycle>> ExecuteAsync(CancellationToken cancellationToken, string? licensePlate = null)
        {
            LogInformation($"Iniciando busca por motocicletas. Filtro de placa: {(string.IsNullOrWhiteSpace(licensePlate) ? "Nenhum" : licensePlate)}");

            try
            {
                var motorcycles = await _repository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(licensePlate))
                {
                    LogInformation($"Filtrando motocicletas pela placa: {licensePlate}");
                    motorcycles = motorcycles.Where(m => m.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));
                }

                LogInformation($"Busca concluída. Total de motocicletas encontradas: {motorcycles.Count()}");

                return motorcycles;
            }
            catch (Exception ex)
            {
                LogError(ex, "Erro ao buscar motocicletas.");
                throw;
            }
        }
    }
}
