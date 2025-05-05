using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IBikeService
{
    Task<IEnumerable<BikeDTO>> GetAllBikesAsync();
    Task<BikeDTO?> GetBikeByIdAsync(Guid id);
    Task<BikeDTO> CreateBikeAsync(CreateBikeDTO createBikeDto);
    Task<BikeDTO?> UpdateBikeAsync(Guid id, UpdateBikeDTO updateBikeDto);
    Task<bool> DeleteBikeAsync(Guid id);
    Task<IEnumerable<BikeDTO>> GetAvailableBikesAsync();
    Task<IEnumerable<BikeDTO>> GetBikesByZoneAsync(Guid zoneId);
    Task<BikeDTO?> AssignZoneAsync(Guid bikeId, Guid zoneId);
}
