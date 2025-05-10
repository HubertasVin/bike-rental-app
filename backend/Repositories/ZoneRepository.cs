using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class ZoneRepository : Repository<Zone>, IZoneRepository
{
    public ZoneRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Zone>> GetAllAsync()
    {
        return await GetAll().Include(z => z.Bikes).ToListAsync();
    }

    public new async Task<Zone?> GetByIdAsync(Guid id)
    {
        return await GetAll().Include(z => z.Bikes).FirstOrDefaultAsync(z => z.Id == id);
    }

    public async Task<Zone> CreateAsync(Zone zone)
    {
        await AddAsync(zone);
        await SaveChangesAsync();
        return zone;
    }

    public new async Task<Zone?> UpdateAsync(Zone zone)
    {
        var existingZone = await GetByIdAsync(zone.Id);
        if (existingZone == null)
        {
            return null;
        }

        _context.Entry(existingZone).CurrentValues.SetValues(zone);
        await SaveChangesAsync();
        return zone;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var zone = await GetByIdAsync(id);
        if (zone == null)
        {
            return false;
        }

        await DeleteAsync(zone);
        await SaveChangesAsync();
        return true;
    }
}
