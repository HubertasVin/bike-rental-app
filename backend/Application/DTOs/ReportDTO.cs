using System;

namespace BikeRentalApp.Application.DTOs;

public class ReportDTO
{
    public Guid Id { get; set; }
    public Guid BikeId { get; set; }
    public Guid? UserId { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
}