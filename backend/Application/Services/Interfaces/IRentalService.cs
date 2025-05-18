using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IRentalService
{
    Task<RentalDTO?> CreateRentalFromReservationAsync(Guid userId, Guid reservationId);
    Task<RentalDTO?> GetRentalByIdAsync(Guid id);
    Task<IEnumerable<RentalDTO>> GetAllRentalsAsync();
    Task<RentalDTO?> GetActiveRentalForUserAsync(Guid userId);
    Task<bool> EndRentalAsync(Guid rentalId, Guid userId, Guid endZoneId);
    Task<bool> DeleteRentalAsync(Guid id);
    Task<RentalDTO?> LockBikeAsync(Guid rentalId, Guid userId);
    Task<RentalDTO?> UnlockBikeAsync(Guid rentalId, Guid userId);
    Task<decimal?> CalculateRentalCostAsync(Guid rentalId);
    Task<bool> IsInAllowedZoneAsync(Guid rentalId, Guid zoneId);
}
