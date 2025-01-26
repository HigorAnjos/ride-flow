using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IDeliveryPersonRepository
    {
        Task AddAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken);
        Task<DeliveryPerson> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
        Task<IEnumerable<DeliveryPerson>> GetAllAsync(CancellationToken cancellationToken);
        Task UpdateLicenseImageAsync(DeliveryPerson deliveryPerson, CancellationToken cancellationToken);
    }
}
