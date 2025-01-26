using Application.Abstractions.Repositories;
using Application.Enumerations;
using Domain.Entities;
using Infrastructure.Storage.Repositories.Base;
using Infrastructure.Storage.Services;
using System.Text.Json;
using Dapper;

namespace Infrastructure.Storage.Repositories
{
    public class DeliveryPersonRepository : RepositoryBase, IDeliveryPersonRepository
    {
        private readonly IImageStorageService _imageStorageService;
        private string BaseDirectory => Path.Combine(AppContext.BaseDirectory, "Storage", "DeliveryPersons");
        private string DataFilePath => Path.Combine(BaseDirectory, "delivery-persons.json");


        private IScriptLoader _scriptLoader { get; set; }
        private IConnectionFactory _connectionFactory { get; set; }
        protected override string FolderPath => "Infrastructure.Storage.Scripts.DeliveryPerson";


        public DeliveryPersonRepository(IImageStorageService imageStorageService, IScriptLoader scriptLoader, IConnectionFactory connectionFactory)
        {
            _imageStorageService = imageStorageService ?? throw new ArgumentNullException(nameof(imageStorageService));
            Directory.CreateDirectory(BaseDirectory); // Garante que o diretório base exista

            if (!File.Exists(DataFilePath))
            {
                File.WriteAllText(DataFilePath, JsonSerializer.Serialize(new List<DeliveryPerson>()));
            }

            _scriptLoader = scriptLoader;
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken)
        {
            if (deliveryPerson == null)
                throw new ArgumentNullException(nameof(deliveryPerson));

            var deliveryPersons = await GetAllInternalAsync();

            // Verifica se o entregador já existe
            var existingPerson = deliveryPersons.Find(d => d.Id == deliveryPerson.Id);
            if (existingPerson != null)
            {
                deliveryPersons.Remove(existingPerson);
            }

            // Salva a imagem no armazenamento local
            if (!string.IsNullOrWhiteSpace(deliveryPerson.LicenseImage))
            {
                var relativePath = await _imageStorageService.SaveImageAsync(deliveryPerson.Id, deliveryPerson.LicenseImage, "LicenseImage");
                deliveryPerson.LicenseImage = relativePath; // Armazena apenas o caminho relativo
            }

            // Adiciona ou atualiza o entregador na lista
            deliveryPersons.Add(deliveryPerson);
            await File.WriteAllTextAsync(DataFilePath, JsonSerializer.Serialize(deliveryPersons, new JsonSerializerOptions { WriteIndented = true }));

            // Salva no banco
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);
            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Add.sql");
            await conn.ExecuteScalarAsync<string>(new CommandDefinition(
                commandText: query,
                parameters: new
                {
                    Id = deliveryPerson.Id,
                    Name = deliveryPerson.Name,
                    CNPJ = deliveryPerson.CNPJ,
                    DateOfBirth = deliveryPerson.DateOfBirth,
                    LicenseNumber = deliveryPerson.LicenseNumber,
                    LicenseType = deliveryPerson.LicenseType
                }));
        }

        public async Task UpdateLicenseImageAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken)
        {
            if (deliveryPerson == null || string.IsNullOrWhiteSpace(deliveryPerson.Id))
                throw new ArgumentException("O entregador ou o ID do entregador não podem ser nulos ou vazios.", nameof(deliveryPerson));

            if (string.IsNullOrWhiteSpace(deliveryPerson.LicenseImage))
                throw new ArgumentException("A imagem da CNH não pode ser vazia.", nameof(deliveryPerson.LicenseImage));

            // Salva a nova imagem da CNH no sistema de arquivos
            var relativePath = await _imageStorageService.SaveImageAsync(deliveryPerson.Id, deliveryPerson.LicenseImage, "LicenseImage");

            // Atualiza o objeto em memória
            var deliveryPersons = await GetAllInternalAsync();
            var existingPerson = deliveryPersons.Find(d => d.Id == deliveryPerson.Id);

            if (existingPerson != null)
            {
                deliveryPersons.Remove(existingPerson);
            }

            // Atualiza o caminho da imagem no objeto deliveryPerson
            deliveryPerson.LicenseImage = relativePath;

            // Adiciona o objeto atualizado na lista
            deliveryPersons.Add(deliveryPerson);

            // Persiste a lista atualizada no arquivo JSON
            await File.WriteAllTextAsync(DataFilePath, JsonSerializer.Serialize(deliveryPersons, new JsonSerializerOptions { WriteIndented = true }));
        }

        public async Task<DeliveryPerson> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("O ID não pode ser vazio.", nameof(id));

            // Consulta no banco de dados
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);
            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetById.sql");

            var deliveryPerson = await conn.QueryFirstOrDefaultAsync<DeliveryPerson>(
                new CommandDefinition(
                    commandText: query,
                    parameters: new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            if (deliveryPerson == null)
                throw new KeyNotFoundException($"Nenhum entregador encontrado com o ID: {id}");

            // Tenta recuperar a imagem da CNH do armazenamento local
            var base64Image = await _imageStorageService.GetImageAsync(id, "LicenseImage");
            if (!string.IsNullOrWhiteSpace(base64Image))
            {
                deliveryPerson.LicenseImage = base64Image;
            }

            return deliveryPerson;
        }


        public async Task<IEnumerable<DeliveryPerson>> GetAllAsync(CancellationToken cancellationToken)
        {
            // Consulta no banco de dados
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);
            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "GetAll.sql");

            var deliveryPersons = await conn.QueryAsync<DeliveryPerson>(
                new CommandDefinition(
                    commandText: query,
                    cancellationToken: cancellationToken
                )
            );

            // Recupera as imagens da CNH do armazenamento local
            foreach (var deliveryPerson in deliveryPersons)
            {
                var base64Image = await _imageStorageService.GetImageAsync(deliveryPerson.Id, "LicenseImage");
                if (!string.IsNullOrWhiteSpace(base64Image))
                {
                    deliveryPerson.LicenseImage = base64Image;
                }
            }

            return deliveryPersons;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("O ID não pode ser vazio.", nameof(id));

            // Verifica se o entregador existe no banco de dados
            using var conn = _connectionFactory.CreateConnection(Databases.RIDE_FLOW_POSTGRES);
            var query = await _scriptLoader.GetCachedScriptAsync(FolderPath, "Delete.sql");

            var rowsAffected = await conn.ExecuteAsync(new CommandDefinition(
                commandText: query,
                parameters: new { Id = id },
                cancellationToken: cancellationToken
            ));

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Nenhum entregador encontrado com o ID: {id}");

            // Remove a imagem associada do sistema de arquivos
            await _imageStorageService.DeleteImageAsync(id, "LicenseImage");
        }


        private async Task<List<DeliveryPerson>> GetAllInternalAsync()
        {
            if (!File.Exists(DataFilePath))
            {
                return new List<DeliveryPerson>();
            }

            var json = await File.ReadAllTextAsync(DataFilePath);
            return JsonSerializer.Deserialize<List<DeliveryPerson>>(json) ?? new List<DeliveryPerson>();
        }
    }
}
