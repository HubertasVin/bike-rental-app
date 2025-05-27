using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Include(r => r.StartZone)
            .Include(r => r.EndZone)
            .ToListAsync();
    }

    public new async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Include(r => r.StartZone)
            .Include(r => r.EndZone)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rental> CreateAsync(Rental rental)
    {
        await AddAsync(rental);
        await SaveChangesAsync();
        return rental;
    }

    public new async Task<Rental?> UpdateAsync(Rental rental)
    {
        var existingRental = await GetByIdAsync(rental.Id);
        if (existingRental == null)
        {
            return null;
        }

        _context.Entry(existingRental).CurrentValues.SetValues(rental);
        await SaveChangesAsync();
        return await GetByIdAsync(rental.Id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var count = await _context.Rentals.Where(z => z.Id == id).ExecuteDeleteAsync();
        return count > 0;
    }

    public async Task<Rental?> GetActiveRentalForUserAsync(Guid userId)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Include(r => r.StartZone)
            .Include(r => r.EndZone)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EndTime == null);
    }

    public async Task<bool> HasActiveRentalForBikeAsync(Guid bikeId)
    {
        return await GetAll().AnyAsync(r => r.BikeId == bikeId && r.EndTime == null);
    }
}
