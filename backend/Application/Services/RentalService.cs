using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Domain.Enums;
using BikeRentalApp.Repositories.Interfaces;

namespace BikeRentalApp.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IZoneRepository _zoneRepository;
    private readonly IPricingStrategy _pricingStrategy;

    public RentalService(
        IRentalRepository rentalRepository,
        IBikeRepository bikeRepository,
        IReservationRepository reservationRepository,
        IZoneRepository zoneRepository,
        IPricingStrategy pricingStrategy
    )
    {
        _rentalRepository = rentalRepository;
        _bikeRepository = bikeRepository;
        _reservationRepository = reservationRepository;
        _zoneRepository = zoneRepository;
        _pricingStrategy = pricingStrategy;
    }

    public async Task<RentalDTO?> CreateRentalFromReservationAsync(Guid userId, Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null || reservation.UserId != userId || reservation.EndTime != null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(reservation.BikeId);
        if (bike == null || bike.Status != BikeStatus.Reserved)
        {
            return null;
        }

        reservation.End();
        await _reservationRepository.UpdateAsync(reservation);

        var rental = new Rental(
            Guid.NewGuid(),
            userId,
            bike.Id,
            bike.ZoneId,
            DateTime.UtcNow,
            reservationId
        );

        var createdRental = await _rentalRepository.CreateAsync(rental);

        var updatedBike = new Bike(
            bike.Id,
            bike.PricePerMinute,
            bike.Model,
            BikeStatus.Rented,
            LockStatus.Unlocked,
            bike.ZoneId
        );

        await _bikeRepository.UpdateAsync(updatedBike);

        return MapToDto(createdRental, updatedBike, null);
    }

    public async Task<RentalDTO?> GetRentalByIdAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null)
        {
            return null;
        }

        var cost = rental.EndTime.HasValue ? await CalculateRentalCostAsync(rental.Id) : null;
        return MapToDto(rental, bike, cost);
    }

    public async Task<IEnumerable<RentalDTO>> GetAllRentalsAsync()
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var result = new List<RentalDTO>();

        foreach (var rental in rentals)
        {
            var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
            if (bike != null)
            {
                var cost = rental.EndTime.HasValue
                    ? await CalculateRentalCostAsync(rental.Id)
                    : null;
                result.Add(MapToDto(rental, bike, cost));
            }
        }

        return result;
    }

    public async Task<RentalDTO?> GetActiveRentalForUserAsync(Guid userId)
    {
        var rental = await _rentalRepository.GetActiveRentalForUserAsync(userId);
        if (rental == null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null)
        {
            return null;
        }

        return MapToDto(rental, bike, null);
    }

    public async Task<bool> EndRentalAsync(Guid rentalId, Guid userId, Guid endZoneId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null || rental.UserId != userId || rental.EndTime != null)
        {
            return false;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null)
        {
            return false;
        }

        rental.End(endZoneId);
        await _rentalRepository.UpdateAsync(rental);

        var updatedBike = new Bike(
            bike.Id,
            bike.PricePerMinute,
            bike.Model,
            BikeStatus.Available,
            LockStatus.Locked,
            endZoneId
        );

        await _bikeRepository.UpdateAsync(updatedBike);

        return true;
    }

    public async Task<bool> DeleteRentalAsync(Guid id)
    {
        return await _rentalRepository.DeleteAsync(id);
    }

    public async Task<RentalDTO?> LockBikeAsync(Guid rentalId, Guid userId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null || rental.UserId != userId || rental.EndTime != null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null || bike.Status != BikeStatus.Rented)
        {
            return null;
        }

        var updatedBike = new Bike(
            bike.Id,
            bike.PricePerMinute,
            bike.Model,
            bike.Status,
            LockStatus.Locked,
            bike.ZoneId
        );

        await _bikeRepository.UpdateAsync(updatedBike);

        return MapToDto(rental, updatedBike, null);
    }

    public async Task<RentalDTO?> UnlockBikeAsync(Guid rentalId, Guid userId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null || rental.UserId != userId || rental.EndTime != null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null || bike.Status != BikeStatus.Rented)
        {
            return null;
        }

        var updatedBike = new Bike(
            bike.Id,
            bike.PricePerMinute,
            bike.Model,
            bike.Status,
            LockStatus.Unlocked,
            bike.ZoneId
        );

        await _bikeRepository.UpdateAsync(updatedBike);

        return MapToDto(rental, updatedBike, null);
    }

    public async Task<decimal?> CalculateRentalCostAsync(Guid rentalId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
        {
            return null;
        }

        var bike = await _bikeRepository.GetByIdAsync(rental.BikeId);
        if (bike == null)
        {
            return null;
        }

        var duration = rental.EndTime.HasValue
            ? rental.EndTime.Value - rental.StartTime
            : DateTime.UtcNow - rental.StartTime;

        return _pricingStrategy.CalculatePrice(bike.PricePerMinute, duration.TotalMinutes);
    }

    public async Task<bool> IsInAllowedZoneAsync(Guid rentalId, Guid zoneId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
        {
            return false;
        }

        var zone = await _zoneRepository.GetByIdAsync(zoneId);
        if (zone == null)
        {
            return false;
        }

        // Assuming all existing zones are valid
        return true;
    }

    private RentalDTO MapToDto(Rental rental, Bike bike, decimal? cost)
    {
        return new RentalDTO
        {
            Id = rental.Id,
            UserId = rental.UserId,
            BikeId = rental.BikeId,
            BikeModel = bike.Model,
            StartTime = rental.StartTime,
            EndTime = rental.EndTime,
            StartZoneId = rental.StartZoneId,
            StartZoneName = rental.StartZone?.Name ?? "Unknown",
            EndZoneId = rental.EndZoneId,
            EndZoneName = rental.EndZone?.Name,
            BikeStatus = bike.Status.ToString(),
            LockStatus = bike.LockStatus.ToString(),
            Cost = cost,
            ReservationId = rental.ReservationId
        };
    }
}
