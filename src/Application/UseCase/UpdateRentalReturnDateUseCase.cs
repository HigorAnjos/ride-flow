using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;

namespace Application.UseCase
{
    public interface IUpdateRentalReturnDateUseCase
    {
        Task<decimal> ExecuteAsync(string rentalId, DateTime returnDate, CancellationToken cancellationToken);
    }

    public class UpdateRentalReturnDateUseCase : BaseUseCase<UpdateRentalReturnDateUseCase>, IUpdateRentalReturnDateUseCase
    {
        private readonly IRentalRepository _rentalRepository;
        protected override string ActionIdentification { get; } = "UpdateRentalReturnDateUseCase";


        public UpdateRentalReturnDateUseCase(IRentalRepository rentalRepository, ILoggerService<UpdateRentalReturnDateUseCase> logger)
            : base(logger)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        }

        public async Task<decimal> ExecuteAsync(string rentalId, DateTime returnDate, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando atualização da data de devolução. Rental ID: {rentalId}, Data de Devolução: {returnDate:yyyy-MM-dd}");

            if (string.IsNullOrWhiteSpace(rentalId))
            {
                LogWarning("O ID da locação está vazio ou nulo.");
                throw new ArgumentException("Rental ID cannot be null or empty.", nameof(rentalId));
            }

            if (returnDate == default)
            {
                LogWarning("A data de devolução fornecida é inválida.");
                throw new InvalidDataException("Return date is invalid.");
            }

            var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);

            if (rental == null)
            {
                LogWarning($"Nenhuma locação encontrada com o ID: {rentalId}");
                throw new Exception($"Rental with ID '{rentalId}' was not found.");
            }

            try
            {
                rental.SetReturnDate(returnDate);
                LogInformation($"Data de devolução definida com sucesso. Rental ID: {rental.Id}, Data de Devolução: {returnDate:yyyy-MM-dd}");

                if (!rental.IsValid())
                {
                    LogWarning($"Locação inválida após definir a data de devolução. Rental ID: {rental.Id}");
                    throw new InvalidDataException("The rental contains invalid data after setting the return date.");
                }

                await _rentalRepository.UpdateReturnDateAsync(rental, cancellationToken);
                LogInformation($"Locação salva com sucesso após atualização. Rental ID: {rental.Id}");

                var finalCost = rental.CalculateFinalCost();
                LogInformation($"Custo final calculado. Rental ID: {rental.Id}, Custo Final: {finalCost:C}");

                return finalCost;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao atualizar a data de devolução. Rental ID: {rentalId}");
                throw;
            }
        }
    }
}
