using System;

namespace BikeRentalApp.Application.DTOs;

public class ReservationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BikeId { get; set; }
    public string BikeModel { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive => EndTime == null;
    public TimeSpan ReservationDuration =>
        EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
}
