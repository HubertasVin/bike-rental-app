using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Report>> GetAllAsync()
    {
        return await GetAll().Include(r => r.Bike).Include(r => r.User).ToListAsync();
    }

    public new async Task<Report?> GetByIdAsync(Guid id)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report> CreateAsync(Report report)
    {
        await AddAsync(report);
        await SaveChangesAsync();
        return report;
    }

    public new async Task<Report?> UpdateAsync(Report report)
    {
        var existingReport = await GetByIdAsync(report.Id);
        if (existingReport == null)
        {
            return null;
        }

        _context.Entry(existingReport).CurrentValues.SetValues(report);
        await SaveChangesAsync();
        return report;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var count = await _context.Reports.Where(r => r.Id == id).ExecuteDeleteAsync();
        return count > 0;

        // Original code, that was improved
        // var report = await GetByIdAsync(id);
        // if (report == null)
        // {
        //     return false;
        // }

        // await DeleteAsync(report);
        // await SaveChangesAsync();
        // return true;
    }

    public async Task<IEnumerable<Report>> GetReportsByBikeAsync(Guid bikeId)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Where(r => r.BikeId == bikeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByUserAsync(Guid userId)
    {
        return await GetAll()
            .Include(r => r.Bike)
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}
