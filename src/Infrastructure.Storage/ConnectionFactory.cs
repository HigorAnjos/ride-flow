using Application.Abstractions.Repositories;
using Application.Enumerations;
using Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Npgsql;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Storage
{
    [ExcludeFromCodeCoverage]
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IMongoClient _mongoClient;
        private readonly IOptionsSnapshot<RideFlowSqlServerOptions> _options;

        public ConnectionFactory(IOptionsSnapshot<RideFlowSqlServerOptions> options, IMongoClient mongoClient)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        }

        public IDbConnection CreateConnection(Databases connectionDataBase)
        {
            return connectionDataBase switch
            {
                Databases.RIDE_FLOW_POSTGRES => new NpgsqlConnection(_options.Value.RideFlowSqlPostgres),
                _ => throw new NotSupportedException($"Conexão para o banco de dados {connectionDataBase} não é suportada.")
            };
        }

        public IMongoDatabase CreateMongoDatabase(Databases connectionDataBase, string databaseName)
        {
            switch (connectionDataBase)
            {
                case Databases.RIDE_FLOW_MONGODB:
                    if (string.IsNullOrWhiteSpace(databaseName))
                        throw new ArgumentException("O nome do banco de dados MongoDB não pode ser vazio.", nameof(databaseName));

                    return _mongoClient.GetDatabase(databaseName);

                default:
                    throw new NotSupportedException($"Conexão para o banco de dados {connectionDataBase} não é suportada.");
            }
        }
    }
}
