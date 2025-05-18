using BikeRentalApp.Application.DTOs;

namespace BikeRentalApp.Application.Services.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDTO>> GetAllReportsAsync();
    Task<ReportDTO?> GetReportByIdAsync(Guid id);
    Task<ReportDTO> CreateReportAsync(CreateReportDTO createReportDto);
    Task<ReportDTO?> UpdateReportAsync(Guid id, UpdateReportDTO updateReportDto);
    Task<bool> DeleteReportAsync(Guid id);
    Task<IEnumerable<ReportDTO>> GetReportsByBikeAsync(Guid bikeId);
    Task<IEnumerable<ReportDTO>> GetReportsByUserAsync(Guid userId);
}