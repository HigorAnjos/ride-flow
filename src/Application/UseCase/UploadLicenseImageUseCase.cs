using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;

namespace Application.UseCase
{
    public interface IUploadLicenseImageUseCase
    {
        Task ExecuteAsync(string deliveryPersonId, string licenseImageBase64, CancellationToken cancellationToken);
    }

    public class UploadLicenseImageUseCase : BaseUseCase<UploadLicenseImageUseCase>, IUploadLicenseImageUseCase
    {
        private readonly IDeliveryPersonRepository _repository;
        protected override string ActionIdentification { get; } = "UploadLicenseImageUseCase";

        public UploadLicenseImageUseCase(IDeliveryPersonRepository repository, ILoggerService<UploadLicenseImageUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task ExecuteAsync(string deliveryPersonId, string licenseImageBase64, CancellationToken cancellationToken)
        {
            LogInformation($"Iniciando upload da imagem da CNH. Entregador ID: {deliveryPersonId}");

            // Validações iniciais
            if (string.IsNullOrWhiteSpace(deliveryPersonId))
            {
                LogWarning("O ID do entregador está vazio ou nulo.");
                throw new ArgumentException("O ID do entregador é obrigatório.", nameof(deliveryPersonId));
            }

            if (string.IsNullOrWhiteSpace(licenseImageBase64))
            {
                LogWarning("A imagem da CNH está vazia ou nula.");
                throw new ArgumentException("A imagem da CNH é obrigatória.", nameof(licenseImageBase64));
            }

            // Busca o entregador no repositório
            var deliveryPerson = await _repository.GetByIdAsync(deliveryPersonId, cancellationToken);
            if (deliveryPerson == null)
            {
                LogWarning($"Entregador não encontrado. Entregador ID: {deliveryPersonId}");
                throw new InvalidOperationException("Entregador não encontrado.");
            }

            try
            {
                // Atualiza a imagem da CNH
                LogInformation($"Atualizando a imagem da CNH no repositório. Entregador ID: {deliveryPersonId}");
                deliveryPerson.LicenseImage = licenseImageBase64;
                await _repository.UpdateLicenseImageAsync(deliveryPerson, cancellationToken);
                LogInformation($"Imagem da CNH atualizada com sucesso. Entregador ID: {deliveryPersonId}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Erro ao atualizar a imagem da CNH. Entregador ID: {deliveryPersonId}");
                throw;
            }
        }
    }
}
