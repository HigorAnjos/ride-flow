using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IMotorcycleRepository
    {
        Task AddAsync(Motorcycle motorcycle, CancellationToken cancellationToken);
        Task UpdateAsync(Motorcycle motorcycle, CancellationToken cancellationToken);
        Task<Motorcycle> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<Motorcycle> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken);
        Task<IEnumerable<Motorcycle>> GetAllAsync(CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }
}
