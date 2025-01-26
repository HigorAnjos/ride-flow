using Application.Abstractions.Repositories;
using Application.Enumerations;
using Dapper;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Plans;
using Domain.Plans.Base;
using Infrastructure.Storage.Repositories.Base;

namespace Infrastructure.Storage.Repositories
{
    public class RentalRepository : RepositoryBase, IRentalRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IScriptLoader _scriptLoader;
        private readonly IRentalPlanFactory _rentalPlanFactory;

        protected override string FolderPath => "Infrastructure.Storage.Scripts.Rental";

        public RentalRepository(
            IConnectionFactory connectionFactory,
            IScriptLoader scriptLoader,
            IRentalPlanFactory rentalPlanFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _scriptLoader = scriptLoader ?? throw new ArgumentNullException(nameof(scriptLoader));
            _rentalPlanFactory = rentalPlanFactory ?? throw new ArgumentNullException(nameof(rentalPlanFactory));
        }

        public async Task<Rental> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("O ID não pode ser vazio.", nameof(id));

            try
            {
                using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

                var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetById.sql");

                var rental = await conn.QueryFirstOrDefaultAsync<Rental>(
                    new CommandDefinition(
                        commandText: query,
                        parameters: new { Id = id },
                        cancellationToken: cancellationToken
                    )
                );

                if (rental == null)
                    throw new KeyNotFoundException($"Nenhuma locação encontrada com o ID: {id}");

                // Configura o RentalPlan usando o RentalPlanFactory
                var rentalPlan = _rentalPlanFactory.Create(rental.RentalPlanType);
                rental.SetRentalPlan(rentalPlan, rental.RentalPlanType);

                return rental;
            }
            catch (Exception ex)
            {
                // Lança uma exceção genérica em caso de falha inesperada
                Console.WriteLine($"Erro ao buscar locação pelo ID {id}: {ex.Message}");
                throw new Exception($"Erro interno ao buscar a locação com ID: {id}.", ex);
            }
        }


        public async Task AddAsync(Rental rental, CancellationToken cancellationToken)
        {
            if (rental == null || !rental.IsValid())
                throw new ArgumentException("Locação inválida.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Add.sql");

            if (!Enum.IsDefined(typeof(RentalPlanTypeEnum), rental.RentalPlanType))
                throw new ArgumentException("Plano de locação inválido.", nameof(rental.RentalPlanType));

            await conn.ExecuteAsync(
                new CommandDefinition(
                    commandText: query,
                    parameters: new
                    {
                        Id = rental.Id,
                        DeliveryPersonId = rental.DeliveryPersonId,
                        MotorcycleId = rental.MotorcycleId,
                        StartDate = rental.StartDate,
                        EndDate = rental.EndDate,
                        ExpectedEndDate = rental.ExpectedEndDate,
                        ReturnDate = rental.ReturnDate,
                        RentalPlan = (int)rental.RentalPlanType
                    },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task UpdateReturnDateAsync(Rental rental, CancellationToken cancellationToken)
        {
            if (rental == null || string.IsNullOrWhiteSpace(rental.Id))
                throw new ArgumentException("A locação fornecida é inválida ou o ID está vazio.", nameof(rental));

            if (!rental.ReturnDate.HasValue || rental.ReturnDate.Value == default)
                throw new ArgumentException("A data de devolução fornecida é inválida.", nameof(rental.ReturnDate));

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "UpdateReturnDate.sql");

            var rowsAffected = await conn.ExecuteAsync(
                new CommandDefinition(
                    commandText: query,
                    parameters: new
                    {
                        Id = rental.Id,
                        ReturnDate = rental.ReturnDate
                    },
                    cancellationToken: cancellationToken
                )
            );

            if (rowsAffected == 0)
                throw new InvalidOperationException($"Nenhuma locação encontrada com o ID: {rental.Id}.");
        }

        public async Task<bool> HasRentalsAsync(string motorcycleId, CancellationToken cancellationToken)
        {
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);
            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "CheckRentals.sql");

            var command = new CommandDefinition(
                commandText: query,
                parameters: new { MotorcycleId = motorcycleId },
                cancellationToken: cancellationToken
            );

            var count = await conn.ExecuteScalarAsync<int>(command);
            return count > 0;
        }
    }
}
