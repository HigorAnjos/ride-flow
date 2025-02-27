﻿using Application.Abstractions.Repositories;
using Application.Enumerations;
using Dapper;
using Domain.Entities;
using Infrastructure.Storage.Repositories.Base;

namespace Infrastructure.Storage.Repositories
{
    public class MotorcycleRepository : RepositoryBase, IMotorcycleRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IScriptLoader _scriptLoader;
        protected override string FolderPath => "Infrastructure.Storage.Scripts.Motorcycle";

        public MotorcycleRepository(IConnectionFactory connectionFactory, IScriptLoader scriptLoader)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _scriptLoader = scriptLoader ?? throw new ArgumentNullException(nameof(scriptLoader));
        }

        public async Task AddAsync(Motorcycle motorcycle, CancellationToken cancellationToken)
        {
            if (motorcycle == null || !motorcycle.IsValid())
                throw new ArgumentException("Moto inválida.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Add.sql");
            await conn.ExecuteAsync(new CommandDefinition(
                commandText: query,
                parameters: new
                {
                    Id = motorcycle.Id,
                    Year = motorcycle.Year,
                    Model = motorcycle.Model,
                    LicensePlate = motorcycle.LicensePlate
                },
                cancellationToken: cancellationToken
            ));
        }

        public async Task<Motorcycle> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("O ID não pode ser vazio.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetById.sql");
            return await conn.QueryFirstOrDefaultAsync<Motorcycle>(new CommandDefinition(
                commandText: query,
                parameters: new { Id = id },
                cancellationToken: cancellationToken
            ));
        }

        public async Task<Motorcycle> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("A placa não pode ser vazia.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetByLicensePlate.sql");
            return await conn.QueryFirstOrDefaultAsync<Motorcycle>(new CommandDefinition(
                commandText: query,
                parameters: new { LicensePlate = licensePlate },
                cancellationToken: cancellationToken
            ));
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync(CancellationToken cancellationToken)
        {
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetAll.sql");

            return await conn.QueryAsync<Motorcycle>(new CommandDefinition(
                commandText: query,
                cancellationToken: cancellationToken
            ));
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("O ID não pode ser vazio.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Delete.sql");
            await conn.ExecuteAsync(new CommandDefinition(
                commandText: query,
                parameters: new { Id = id },
                cancellationToken: cancellationToken
            ));
        }

        public async Task UpdateAsync(Motorcycle motorcycle, CancellationToken cancellationToken)
        {
            if (motorcycle == null || !motorcycle.IsValid())
                throw new ArgumentException("Moto inválida.");

            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);

            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Update.sql");

            await conn.ExecuteAsync(new CommandDefinition(
                commandText: query,
                parameters: new
                {
                    Id = motorcycle.Id,
                    Year = motorcycle.Year,
                    Model = motorcycle.Model,
                    LicensePlate = motorcycle.LicensePlate
                },
                cancellationToken: cancellationToken
            ));
        }
    }
}
