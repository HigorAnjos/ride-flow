using Application.Abstractions.Logging;
using Application.Abstractions.Repositories;
using Application.UseCase.Base;
using Domain.Entities;

namespace Application.UseCase
{
    public interface ICreateDeliveryPersonUseCase
    {
        Task ExecuteAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken);
    }

    public class CreateDeliveryPersonUseCase : BaseUseCase<CreateDeliveryPersonUseCase>, ICreateDeliveryPersonUseCase
    {
        private readonly IDeliveryPersonRepository _repository;

        protected override string ActionIdentification { get; } = "CreateDeliveryPersonUseCase";

        public CreateDeliveryPersonUseCase(IDeliveryPersonRepository repository, ILoggerService<CreateDeliveryPersonUseCase> logger)
            : base(logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task ExecuteAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken)
        {
            LogInformation("Iniciando a criação de um novo entregador.");

            if (deliveryPerson == null)
            {
                LogError("Os dados do entregador são nulos.");
                throw new ArgumentNullException(nameof(deliveryPerson), "Os dados do entregador são obrigatórios.");
            }

            if (!deliveryPerson.IsValid())
            {
                LogWarning("Os dados do entregador são inválidos.");
                throw new ArgumentException("Dados inválidos.");
            }

            try
            {
                LogInformation("Salvando o entregador no repositório. Nome: {Name}, CNPJ: {CNPJ}", deliveryPerson.Name, deliveryPerson.CNPJ);
                await _repository.AddAsync(deliveryPerson, cancellationToken);
                LogInformation("Entregador salvo com sucesso. Nome: {Name}, CNPJ: {CNPJ}", deliveryPerson.Name, deliveryPerson.CNPJ);
            }
            catch (Exception ex)
            {
                LogError(ex, "Erro ao salvar o entregador. Nome: {Name}, CNPJ: {CNPJ}", deliveryPerson.Name, deliveryPerson.CNPJ);
                throw;
            }
        }
    }
}
