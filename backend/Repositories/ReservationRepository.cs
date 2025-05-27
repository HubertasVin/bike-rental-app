using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await GetAll().Include(r => r.Bike).Include(r => r.User).ToListAsync();
    }

    public new async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        await AddAsync(reservation);
        await SaveChangesAsync();
        return reservation;
    }

    public new async Task<Reservation?> UpdateAsync(Reservation reservation)
    {
        var existingReservation = await GetByIdAsync(reservation.Id);
        if (existingReservation == null)
        {
            return null;
        }

        _context.Entry(existingReservation).CurrentValues.SetValues(reservation);
        await SaveChangesAsync();
        return reservation;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var count = await _context.Reservations.Where(r => r.Id == id).ExecuteDeleteAsync();
        return count > 0;
    }

    public async Task<Reservation?> GetActiveReservationForUserAsync(Guid userId)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EndTime == null);
    }

    public async Task<bool> HasActiveReservationForBikeAsync(Guid bikeId)
    {
        return await GetAll().AnyAsync(r => r.BikeId == bikeId && r.EndTime == null);
    }
}
