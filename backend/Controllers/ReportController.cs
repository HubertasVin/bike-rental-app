using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeRentalApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Gets all reports
    /// </summary>
    /// <response code="200">List of all reports</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    /// <summary>
    /// Gets a report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <response code="200">Report details</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }

        return Ok(report);
    }

    /// <summary>
    /// Creates a new report
    /// </summary>
    /// <param name="createReportDto">New report details</param>
    /// <response code="201">Created report</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateReportDTO createReportDto)
    {
        var createdReport = await _reportService.CreateReportAsync(createReportDto);
        return CreatedAtAction(nameof(GetById), new { id = createdReport.Id }, createdReport);
    }

    /// <summary>
    /// Updates an existing report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="updateReportDto">Updated report details</param>
    /// <response code="200">Updated report</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateReportDTO updateReportDto)
    {
        try
        {
            var updatedReport = await _reportService.UpdateReportAsync(id, updateReportDto);
            if (updatedReport == null)
            {
                return NotFound();
            }

            return Ok(updatedReport);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a report
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <response code="204">No content</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all reports for a specific bike
    /// </summary>
    /// <param name="bikeId">Bike ID</param>
    /// <response code="200">List of reports for the bike</response>
    [HttpGet("bike/{bikeId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByBike(Guid bikeId)
    {
        var reports = await _reportService.GetReportsByBikeAsync(bikeId);
        return Ok(reports);
    }

    /// <summary>
    /// Gets all reports submitted by a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <response code="200">List of reports by the user</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var reports = await _reportService.GetReportsByUserAsync(userId);
        return Ok(reports);
    }
}