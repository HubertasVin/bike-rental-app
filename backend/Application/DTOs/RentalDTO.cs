namespace BikeRentalApp.Application.DTOs;

public class RentalDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BikeId { get; set; }
    public string BikeModel { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive => EndTime == null;
    public TimeSpan RentalDuration =>
        EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
    public Guid StartZoneId { get; set; }
    public string StartZoneName { get; set; } = null!;
    public Guid? EndZoneId { get; set; }
    public string? EndZoneName { get; set; }
    public string BikeStatus { get; set; } = null!;
    public string LockStatus { get; set; } = null!;
    public decimal? Cost { get; set; }
    public Guid ReservationId { get; set; }
}
