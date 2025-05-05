// Application/Services/ZoneService.cs
using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;

namespace BikeRentalApp.Application.Services;

public class ZoneService : IZoneService
{
    private readonly IZoneRepository _zoneRepository;

    public ZoneService(IZoneRepository zoneRepository)
    {
        _zoneRepository = zoneRepository;
    }

    public async Task<IEnumerable<ZoneDTO>> GetAllZonesAsync()
        => (await _zoneRepository.GetAllAsync())
           .Select(MapToDto);

    public async Task<ZoneDTO?> GetZoneByIdAsync(Guid id)
        => (await _zoneRepository.GetByIdAsync(id)) is Zone z
           ? MapToDto(z)
           : null;

    public async Task<ZoneDTO> CreateZoneAsync(CreateZoneDTO dto)
    {
        var zone = new Zone(
            Guid.NewGuid(),
            dto.Name,
            dto.Address,
            dto.Latitude1,
            dto.Longitude1,
            dto.Latitude2,
            dto.Longitude2,
            dto.Capacity
        );

        var created = await _zoneRepository.CreateAsync(zone);
        return MapToDto(created);
    }

    public async Task<ZoneDTO?> UpdateZoneAsync(Guid id, UpdateZoneDTO dto)
    {
        var existing = await _zoneRepository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing = new Zone(
            existing.Id,
            dto.Name,
            dto.Address,
            dto.Latitude1,
            dto.Longitude1,
            dto.Latitude2,
            dto.Longitude2,
            dto.Capacity
        );

        var updated = await _zoneRepository.UpdateAsync(existing);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteZoneAsync(Guid id)
        => await _zoneRepository.DeleteAsync(id);

    private static ZoneDTO MapToDto(Zone zone)
        => new ZoneDTO
        {
            Id         = zone.Id,
            Name       = zone.Name,
            Address    = zone.Address,
            Latitude1  = zone.Latitude1,
            Longitude1 = zone.Longitude1,
            Latitude2  = zone.Latitude2,
            Longitude2 = zone.Longitude2,
            Capacity   = zone.Capacity,
            Bikes      = zone.Bikes.Select(BikeService.MapToDto).ToList()
        };
}
