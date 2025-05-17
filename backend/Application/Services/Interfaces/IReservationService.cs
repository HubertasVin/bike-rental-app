using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationDTO?> CreateReservationAsync(
        Guid userId,
        CreateReservationDTO createReservationDto
    );
    Task<ReservationDTO?> GetReservationByIdAsync(Guid id);
    Task<IEnumerable<ReservationDTO>> GetUserReservationsAsync(Guid userId);
    Task<ReservationDTO?> GetActiveReservationForUserAsync(Guid userId);
    Task<bool> CancelReservationAsync(Guid reservationId, Guid userId);
    Task<bool> IsReservationFreeAsync(Guid reservationId);
    Task<decimal?> CalculateReservationCostAsync(Guid reservationId);
}
