using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IRentalRepository
    {
        Task AddAsync(Rental rental, CancellationToken cancellationToken);
        Task<Rental> GetByIdAsync(string id, CancellationToken cancellationToken); 
        Task UpdateReturnDateAsync(Rental rental, CancellationToken cancellationToken);
        Task<bool> HasRentalsAsync(string motorcycleId, CancellationToken cancellationToken);
    }
}
