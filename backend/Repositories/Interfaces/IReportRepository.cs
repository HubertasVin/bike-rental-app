using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAllAsync();
    Task<Report?> GetByIdAsync(Guid id);
    Task<Report> CreateAsync(Report report);
    Task<Report?> UpdateAsync(Report report);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Report>> GetReportsByBikeAsync(Guid bikeId);
    Task<IEnumerable<Report>> GetReportsByUserAsync(Guid userId);
}