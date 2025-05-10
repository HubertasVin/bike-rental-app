using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IZoneService
{
    Task<IEnumerable<ZoneDTO>> GetAllZonesAsync();
    Task<IEnumerable<ZoneWithBikesNumberDTO>> GetAllZonesWithBikesCountAsync();
    Task<ZoneDTO?> GetZoneByIdAsync(Guid id);
    Task<ZoneDTO> CreateZoneAsync(CreateZoneDTO dto);
    Task<ZoneDTO?> UpdateZoneAsync(Guid id, UpdateZoneDTO dto);
    Task<bool> DeleteZoneAsync(Guid id);
}
