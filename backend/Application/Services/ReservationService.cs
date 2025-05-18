using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Domain.Enums;
using BikeRentalApp.Repositories.Interfaces;

namespace BikeRentalApp.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly TimeSpan _freeReservationTime = TimeSpan.FromMinutes(10);

    public ReservationService(
        IReservationRepository reservationRepository,
        IBikeRepository bikeRepository
    )
    {
        _reservationRepository = reservationRepository;
        _bikeRepository = bikeRepository;
    }

    public async Task<ReservationDTO?> CreateReservationAsync(
        Guid userId,
        CreateReservationDTO createReservationDto
    )
    {
        var bike = await _bikeRepository.GetByIdAsync(createReservationDto.BikeId);
        if (bike == null || bike.Status != BikeStatus.Available)
        {
            return null;
        }

        var existingReservation = await _reservationRepository.GetActiveReservationForUserAsync(
            userId
        );
        if (existingReservation != null)
        {
            return null;
        }

        var bikeHasReservation = await _reservationRepository.HasActiveReservationForBikeAsync(
            createReservationDto.BikeId
        );
        if (bikeHasReservation)
        {
            return null;
        }

        var reservation = new Reservation(
            Guid.NewGuid(),
            userId,
            createReservationDto.BikeId,
            DateTime.UtcNow
        );

        var createdReservation = await _reservationRepository.CreateAsync(reservation);

        bike = new Bike(
            bike.Id,
            bike.PricePerMinute,
            bike.Model,
            BikeStatus.Reserved,
            bike.LockStatus,
            bike.ZoneId
        );

        await _bikeRepository.UpdateAsync(bike);

        return MapToDto(createdReservation);
    }

    public async Task<ReservationDTO?> GetReservationByIdAsync(Guid id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        return reservation != null ? MapToDto(reservation) : null;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return reservations.Select(MapToDto);
    }

    public async Task<ReservationDTO?> GetActiveReservationForUserAsync(Guid userId)
    {
        var reservation = await _reservationRepository.GetActiveReservationForUserAsync(userId);
        return reservation != null ? MapToDto(reservation) : null;
    }

    public async Task<bool> CancelReservationAsync(Guid reservationId, Guid userId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null || reservation.UserId != userId || reservation.EndTime != null)
        {
            return false;
        }

        reservation.End();
        await _reservationRepository.UpdateAsync(reservation);

        var bike = await _bikeRepository.GetByIdAsync(reservation.BikeId);
        if (bike != null)
        {
            var updatedBike = new Bike(
                bike.Id,
                bike.PricePerMinute,
                bike.Model,
                BikeStatus.Available,
                LockStatus.Locked,
                bike.ZoneId
            );

            await _bikeRepository.UpdateAsync(updatedBike);
        }

        return true;
    }

    public async Task<bool> DeleteReservationAsync(Guid id)
    {
        return await _reservationRepository.DeleteAsync(id);
    }

    public async Task<bool> IsReservationFreeAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
        {
            return false;
        }

        var duration = reservation.EndTime.HasValue
            ? reservation.EndTime.Value - reservation.StartTime
            : DateTime.UtcNow - reservation.StartTime;

        return duration <= _freeReservationTime;
    }

    public async Task<decimal?> CalculateReservationCostAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
        {
            return null;
        }

        if (await IsReservationFreeAsync(reservationId))
        {
            return 0;
        }

        var bike = await _bikeRepository.GetByIdAsync(reservation.BikeId);
        if (bike == null)
        {
            return null;
        }

        var duration = reservation.EndTime.HasValue
            ? reservation.EndTime.Value - reservation.StartTime
            : DateTime.UtcNow - reservation.StartTime;

        var chargeDuration = duration - _freeReservationTime;
        if (chargeDuration.TotalMinutes <= 0)
        {
            return 0;
        }

        return (decimal)Math.Ceiling(chargeDuration.TotalMinutes) * bike.PricePerMinute;
    }

    private static ReservationDTO MapToDto(Reservation reservation)
    {
        return new ReservationDTO
        {
            Id = reservation.Id,
            UserId = reservation.UserId,
            BikeId = reservation.BikeId,
            BikeModel = reservation.Bike.Model,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime
        };
    }
}
