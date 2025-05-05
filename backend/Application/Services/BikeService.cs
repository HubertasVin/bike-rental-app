using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Repositories.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Domain.Enums;
using BikeRentalApp.Repositories;

namespace BikeRentalApp.Application.Services;

public class BikeService : IBikeService
{
    private readonly IBikeRepository _bikeRepository;

    public BikeService(IBikeRepository bikeRepository)
    {
        _bikeRepository = bikeRepository;
    }

    public async Task<IEnumerable<BikeDTO>> GetAllBikesAsync()
    {
        var bikes = await _bikeRepository.GetAllAsync();
        return bikes.Select(MapToDto);
    }

    public async Task<BikeDTO?> GetBikeByIdAsync(Guid id)
    {
        var bike = await _bikeRepository.GetByIdAsync(id);
        return bike != null ? MapToDto(bike) : null;
    }

    public async Task<BikeDTO> CreateBikeAsync(CreateBikeDTO createBikeDto)
    {
        var bike = new Bike(
            Guid.NewGuid(),
            createBikeDto.RentPrice,
            createBikeDto.Model,
            BikeStatus.Available,
            LockStatus.Locked,
            createBikeDto.ZoneId
        );

        var createdBike = await _bikeRepository.CreateAsync(bike);
        return MapToDto(createdBike);
    }

    public async Task<BikeDTO?> UpdateBikeAsync(Guid id, UpdateBikeDTO updateBikeDto)
    {
        var existingBike = await _bikeRepository.GetByIdAsync(id);
        if (existingBike == null)
        {
            return null;
        }

        if (!Enum.TryParse<BikeStatus>(updateBikeDto.Status, out var bikeStatus))
        {
            throw new ArgumentException("Invalid bike status");
        }

        if (!Enum.TryParse<LockStatus>(updateBikeDto.LockStatus, out var lockStatus))
        {
            throw new ArgumentException("Invalid lock status");
        }

        var updatedBike = new Bike(
            existingBike.Id,
            updateBikeDto.RentPrice,
            updateBikeDto.Model,
            bikeStatus,
            lockStatus,
            updateBikeDto.ZoneId
        );

        var result = await _bikeRepository.UpdateAsync(updatedBike);
        return result != null ? MapToDto(result) : null;
    }

    public async Task<bool> DeleteBikeAsync(Guid id)
    {
        return await _bikeRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BikeDTO>> GetAvailableBikesAsync()
    {
        var bikes = await _bikeRepository.GetAvailableBikesAsync();
        return bikes.Select(MapToDto);
    }

    public async Task<IEnumerable<BikeDTO>> GetBikesByZoneAsync(Guid zoneId)
    {
        var bikes = await _bikeRepository.GetBikesByZoneAsync(zoneId);
        return bikes.Select(MapToDto);
    }

    public static BikeDTO MapToDto(Bike bike)
    {
        return new BikeDTO
        {
            Id = bike.Id,
            RentPrice = bike.RentPrice,
            Model = bike.Model,
            Status = bike.Status.ToString(),
            LockStatus = bike.LockStatus.ToString(),
            ZoneId = bike.ZoneId
        };
    }
}
