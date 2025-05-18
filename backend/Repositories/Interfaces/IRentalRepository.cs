using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(Guid id);
    Task<Rental> CreateAsync(Rental rental);
    Task<Rental?> UpdateAsync(Rental rental);
    Task<bool> DeleteAsync(Guid id);
    Task<Rental?> GetActiveRentalForUserAsync(Guid userId);
    Task<bool> HasActiveRentalForBikeAsync(Guid bikeId);
}
