using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Domain.Enums;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class BikeRepository : Repository<Bike>, IBikeRepository
{
    public BikeRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Bike>> GetAllAsync()
    {
        return await GetAll().Include(b => b.Zone).ToListAsync();
    }

    public new async Task<Bike?> GetByIdAsync(Guid id)
    {
        return await GetAll().Include(b => b.Zone).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bike> CreateAsync(Bike bike)
    {
        await AddAsync(bike);
        await SaveChangesAsync();
        return bike;
    }

    public new async Task<Bike?> UpdateAsync(Bike bike)
    {
        var existingBike = await GetByIdAsync(bike.Id);
        if (existingBike == null)
        {
            return null;
        }

        _context.Entry(existingBike).CurrentValues.SetValues(bike);
        await SaveChangesAsync();
        return bike;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var bike = await GetByIdAsync(id);
        if (bike == null)
        {
            return false;
        }

        await DeleteAsync(bike);
        await SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Bike>> GetAvailableBikesAsync()
    {
        return await GetAll()
            .Include(b => b.Zone)
            .Where(b => b.Status == BikeStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bike>> GetBikesByZoneAsync(Guid zoneId)
    {
        return await GetAll().Include(b => b.Zone).Where(b => b.ZoneId == zoneId).ToListAsync();
    }
}
