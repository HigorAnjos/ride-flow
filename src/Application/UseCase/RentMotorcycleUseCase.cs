using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Plans;
using Domain.Plans.Base;

namespace Application.UseCase
{
    public interface IRentMotorcycleUseCase
    {
        Task<Rental> ExecuteAsync(string deliveryPersonId, string motorcycleId, DateTime startDate, RentalPlanTypeEnum planType, string? rentalId = null, CancellationToken cancellationToken = default);
    }

    public class RentMotorcycleUseCase : BaseUseCase<RentMotorcycleUseCase>, IRentMotorcycleUseCase
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IRentalPlanFactory _rentalPlanFactory;

        protected override string ActionIdentification { get; } = "RentMotorcycleUseCase";

        public RentMotorcycleUseCase(
            IRentalRepository rentalRepository,
            IRentalPlanFactory rentalPlanFactory,
            ILoggerService<RentMotorcycleUseCase> logger)
            : base(logger)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
            _rentalPlanFactory = rentalPlanFactory ?? throw new ArgumentNullException(nameof(rentalPlanFactory));
        }

        public async Task<Rental> ExecuteAsync(string deliveryPersonId, string motorcycleId, DateTime startDate, RentalPlanTypeEnum planType, string? rentalId = null, CancellationToken cancellationToken = default)
        {
            LogInformation("Iniciando processo de locação para Entregador ID: {DeliveryPersonId}, Moto ID: {MotorcycleId}, Data de Início: {StartDate:yyyy-MM-dd}, Plano: {PlanType}, Locação ID: {RentalId}",
                deliveryPersonId, motorcycleId, startDate, planType, string.IsNullOrWhiteSpace(rentalId) ? "Novo ID será gerado" : rentalId);

            // Cria o plano utilizando a factory
            var rentalPlan = CreateRentalPlan(planType);

            // Gera o ID apenas se não for fornecido
            var idToUse = string.IsNullOrWhiteSpace(rentalId) ? Guid.NewGuid().ToString() : rentalId;

            // Cria a locação
            var rental = CreateRental(deliveryPersonId, motorcycleId, startDate, idToUse, rentalPlan, planType);

            // Valida e salva a locação
            await SaveRentalAsync(rental, cancellationToken);

            return rental;
        }

        private IRentalPlan CreateRentalPlan(RentalPlanTypeEnum planType)
        {
            LogInformation("Criando plano de locação usando a factory. Tipo de plano: {PlanType}", planType);

            try
            {
                return _rentalPlanFactory.Create(planType);
            }
            catch (Exception ex)
            {
                LogError(ex, "Erro ao criar o plano de locação. Tipo de plano: {PlanType}", planType);
                throw new InvalidDataException("Plano de locação inválido.", ex);
            }
        }

        private Rental CreateRental(string deliveryPersonId, string motorcycleId, DateTime startDate, string rentalId, IRentalPlan rentalPlan, RentalPlanTypeEnum planType)
        {
            LogInformation("Criando objeto de locação. Locação ID: {RentalId}, Entregador ID: {DeliveryPersonId}, Moto ID: {MotorcycleId}", rentalId, deliveryPersonId, motorcycleId);

            var rental = new Rental
            {
                Id = rentalId,
                DeliveryPersonId = deliveryPersonId,
                MotorcycleId = motorcycleId,
                StartDate = startDate
            };

            rental.SetRentalPlan(rentalPlan, planType);

            if (!rental.IsValid())
            {
                LogWarning("Locação inválida. Dados: {@Rental}", rental);
                throw new InvalidDataException("Dados da locação inválidos.");
            }

            return rental;
        }

        private async Task SaveRentalAsync(Rental rental, CancellationToken cancellationToken)
        {
            try
            {
                LogInformation("Salvando locação no repositório. Locação ID: {RentalId}", rental.Id);
                await _rentalRepository.AddAsync(rental, cancellationToken);
                LogInformation("Locação salva com sucesso. Locação ID: {RentalId}", rental.Id);
            }
            catch (Exception ex)
            {
                LogError(ex, "Erro ao salvar a locação. Locação ID: {RentalId}", rental.Id);
                throw;
            }
        }
    }
}
