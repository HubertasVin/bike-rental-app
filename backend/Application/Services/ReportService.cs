using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;

namespace BikeRentalApp.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
    {
        var reports = await _reportRepository.GetAllAsync();
        return reports.Select(MapToDto);
    }

    public async Task<ReportDTO?> GetReportByIdAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        return report != null ? MapToDto(report) : null;
    }

    public async Task<ReportDTO> CreateReportAsync(CreateReportDTO createReportDto)
    {
        var report = new Report(
            Guid.NewGuid(),
            createReportDto.BikeId,
            createReportDto.UserId,
            createReportDto.Type,
            createReportDto.Description
        );

        var createdReport = await _reportRepository.CreateAsync(report);
        return MapToDto(createdReport);
    }

    public async Task<ReportDTO?> UpdateReportAsync(Guid id, UpdateReportDTO updateReportDto)
    {
        var existingReport = await _reportRepository.GetByIdAsync(id);
        if (existingReport == null)
        {
            return null;
        }

        var updatedReport = new Report(
            existingReport.Id,
            updateReportDto.BikeId,
            updateReportDto.UserId,
            updateReportDto.Type,
            updateReportDto.Description
        );

        var result = await _reportRepository.UpdateAsync(updatedReport);
        return result != null ? MapToDto(result) : null;
    }

    public async Task<bool> DeleteReportAsync(Guid id)
    {
        return await _reportRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ReportDTO>> GetReportsByBikeAsync(Guid bikeId)
    {
        var reports = await _reportRepository.GetReportsByBikeAsync(bikeId);
        return reports.Select(MapToDto);
    }

    public async Task<IEnumerable<ReportDTO>> GetReportsByUserAsync(Guid userId)
    {
        var reports = await _reportRepository.GetReportsByUserAsync(userId);
        return reports.Select(MapToDto);
    }

    private static ReportDTO MapToDto(Report report)
    {
        return new ReportDTO
        {
            Id = report.Id,
            BikeId = report.BikeId,
            UserId = report.UserId,
            Type = report.Type,
            Description = report.Description
        };
    }
}