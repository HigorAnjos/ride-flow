using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;

namespace Application.UseCase
{
    public interface IUpdateMotorcycleLicensePlateUseCase
    {
        Task ExecuteAsync(string motorcycleId, string newLicensePlate, CancellationToken cancellationToken);
    }

    public class UpdateMotorcycleLicensePlateUseCase : BaseUseCase<UpdateMotorcycleLicensePlateUseCase>, IUpdateMotorcycleLicensePlateUseCase
    {
        private readonly IMotorcycleRepository _repository;
        protected override string ActionIdentification { get; } = "UpdateMotorcycleLicensePlateUseCase";

        public UpdateMotorcycleLicensePlateUseCase(IMotorcycleRepository repository, ILoggerService<UpdateMotorcycleLicensePlateUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task ExecuteAsync(string motorcycleId, string newLicensePlate, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando atualização da placa. Moto ID: {motorcycleId}, Nova Placa: {newLicensePlate}");

            if (string.IsNullOrWhiteSpace(motorcycleId))
            {
                LogWarning("O ID da moto fornecido está vazio.");
                throw new ArgumentException("O ID da moto é obrigatório.", nameof(motorcycleId));
            }

            if (string.IsNullOrWhiteSpace(newLicensePlate))
            {
                LogWarning("A nova placa fornecida está vazia.");
                throw new ArgumentException("A nova placa não pode ser vazia.", nameof(newLicensePlate));
            }

            // Busca a moto pelo ID
            var motorcycle = await _repository.GetByIdAsync(motorcycleId, cancellationToken);
            if (motorcycle == null)
            {
                LogWarning($"Nenhuma moto encontrada com o ID: {motorcycleId}");
                throw new InvalidDataException($"Moto com ID {motorcycleId} não encontrada.");
            }

            // Verifica se a placa já está em uso
            var existingMotorcycle = await _repository.GetByLicensePlateAsync(newLicensePlate, cancellationToken);
            if (existingMotorcycle != null && existingMotorcycle.Id != motorcycle.Id)
            {
                LogWarning($"A nova placa '{newLicensePlate}' já está em uso por outra moto. Moto existente ID: {existingMotorcycle.Id}");
                throw new InvalidDataException($"A placa '{newLicensePlate}' já está em uso.");
            }

            // Atualiza a placa da moto
            motorcycle.UpdateLicensePlate(newLicensePlate);

            // Validar os dados da motocicleta
            if (!motorcycle.IsValid())
            {
                LogWarning($"Os dados da motocicleta são inválidos. ID: {motorcycle.Id}, Ano: {motorcycle.Year}, Modelo: {motorcycle.Model}, Placa: {motorcycle.LicensePlate}");
                throw new InvalidDataException("Os dados da motocicleta são inválidos.");
            }

            try
            {
                // Salva a moto atualizada no repositório
                LogInformation($"Salvando atualização da placa para a moto com ID: {motorcycleId}");
                await _repository.UpdateAsync(motorcycle, cancellationToken);
                LogInformation($"Placa atualizada com sucesso. Moto ID: {motorcycleId}, Nova Placa: {newLicensePlate}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao salvar a atualização da placa. Moto ID: {motorcycleId}, Nova Placa: {newLicensePlate}");
                throw;
            }
        }
    }
}
