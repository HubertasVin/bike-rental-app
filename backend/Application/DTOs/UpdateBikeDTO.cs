using System;

namespace BikeRentalApp.Application.DTOs;

public class UpdateBikeDTO
{
    public decimal PricePerMinute { get; set; }
    public string Model { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string LockStatus { get; set; } = null!;
    public Guid ZoneId { get; set; }
}
