using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IMotorcycleRepository
    {
        Task AddAsync(Motorcycle motorcycle);
        Task UpdateAsync(Motorcycle motorcycle);
        Task<Motorcycle> GetByIdAsync(string id);
        Task<Motorcycle> GetByLicensePlateAsync(string licensePlate);
        Task<IEnumerable<Motorcycle>> GetAllAsync();
        Task DeleteAsync(string id);
    }
}
