using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IZoneRepository
{
    Task<IEnumerable<Zone>> GetAllAsync();
    Task<Zone?> GetByIdAsync(Guid id);
    Task<Zone> CreateAsync(Zone zone);
    Task<Zone?> UpdateAsync(Zone zone);
    Task<bool> DeleteAsync(Guid id);
}
