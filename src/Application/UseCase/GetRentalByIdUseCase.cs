using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;

namespace Application.UseCase
{
    public interface IGetRentalByIdUseCase
    {
        Task<Rental> ExecuteAsync(string rentalId, CancellationToken cancellationToken);
    }

    public class GetRentalByIdUseCase : BaseUseCase<GetRentalByIdUseCase>, IGetRentalByIdUseCase
    {
        private readonly IRentalRepository _rentalRepository;

        protected override string ActionIdentification { get; } = "GetRentalByIdUseCase";


        public GetRentalByIdUseCase(IRentalRepository rentalRepository, ILoggerService<GetRentalByIdUseCase> logger)
            : base(logger)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        }

        public async Task<Rental> ExecuteAsync(string rentalId, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando a busca da locação. ID: {rentalId}");

            if (string.IsNullOrWhiteSpace(rentalId))
            {
                LogWarning("O ID da locação fornecido está vazio ou nulo.");
                throw new ArgumentException("O ID da locação é obrigatório.", nameof(rentalId));
            }

            try
            {
                LogInformation($"Procurando a locação no repositório. ID: {rentalId}");
                var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);

                if (rental == null)
                {
                    LogWarning($"Nenhuma locação encontrada para o ID: {rentalId}");
                }
                else
                {
                    LogInformation($"Locação encontrada. ID: {rental.Id}, Entregador ID: {rental.DeliveryPersonId}, Moto ID: {rental.MotorcycleId}");
                }

                return rental;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao buscar a locação. ID: {rentalId}");
                throw;
            }
        }
    }
}
