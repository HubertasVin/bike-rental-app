using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories;

public interface IBikeRepository
{
    Task<IEnumerable<Bike>> GetAllAsync();
    Task<Bike?> GetByIdAsync(Guid id);
    Task<Bike> CreateAsync(Bike bike);
    Task<Bike?> UpdateAsync(Bike bike);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Bike>> GetAvailableBikesAsync();
    Task<IEnumerable<Bike>> GetBikesByZoneAsync(Guid zoneId);
}
