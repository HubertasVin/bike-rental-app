using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<Reservation> CreateAsync(Reservation reservation);
    Task<Reservation?> UpdateAsync(Reservation reservation);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Reservation>> GetUserReservationsAsync(Guid userId);
    Task<Reservation?> GetActiveReservationForUserAsync(Guid userId);
    Task<bool> HasActiveReservationForBikeAsync(Guid bikeId);
}
