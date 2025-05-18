using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeRentalApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated properly");
        }
        return userId;
    }

    /// <summary>
    /// Creates a new bike reservation
    /// </summary>
    /// <param name="createReservationDto">Reservation details</param>
    /// <response code="201">Created reservation</response>
    /// <response code="400">If reservation cannot be created</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateReservationDTO createReservationDto)
    {
        var userId = GetCurrentUserId();
        var reservation = await _reservationService.CreateReservationAsync(
            userId,
            createReservationDto
        );

        if (reservation == null)
        {
            return BadRequest(
                "Unable to create reservation. Check if the bike is available and you don't have an active reservation."
            );
        }

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    /// <summary>
    /// Gets a reservation by ID
    /// </summary>
    /// <param name="id">Reservation ID</param>
    /// <response code="200">Reservation details</response>
    /// <response code="404">If reservation not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        if (reservation.UserId != userId)
        {
            return Forbid();
        }

        return Ok(reservation);
    }

    /// <summary>
    /// Gets current user's active reservation
    /// </summary>
    /// <response code="200">Active reservation or null if none exists</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ReservationDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        var userId = GetCurrentUserId();
        var reservation = await _reservationService.GetActiveReservationForUserAsync(userId);
        return Ok(reservation);
    }

    /// <summary>
    /// Gets all reservations
    /// </summary>
    /// <response code="200">List of user's reservations</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Cancels a reservation
    /// </summary>
    /// <param name="id">Reservation ID</param>
    /// <response code="200">Cancellation successful</response>
    /// <response code="404">If reservation not found</response>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = GetCurrentUserId();
        var cancelled = await _reservationService.CancelReservationAsync(id, userId);

        if (!cancelled)
        {
            return NotFound();
        }

        return Ok();
    }

    /// <summary>
    /// Deletes a reservation
    /// </summary>
    /// <param name="id">Reservation ID</param>
    /// <response code="204">No content</response>
    /// <response code="404">If reservation not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reservationService.DeleteReservationAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Checks if a reservation is still within free time
    /// </summary>
    /// <param name="id">Reservation ID</param>
    /// <response code="200">True if reservation is free, false otherwise</response>
    [HttpGet("{id:guid}/is-free")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IsFree(Guid id)
    {
        var isFree = await _reservationService.IsReservationFreeAsync(id);
        return Ok(isFree);
    }

    /// <summary>
    /// Calculates the cost of a reservation
    /// </summary>
    /// <param name="id">Reservation ID</param>
    /// <response code="200">Cost of reservation</response>
    /// <response code="404">If reservation not found</response>
    [HttpGet("{id:guid}/cost")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateCost(Guid id)
    {
        var cost = await _reservationService.CalculateReservationCostAsync(id);

        if (cost == null)
        {
            return NotFound();
        }

        return Ok(cost);
    }
}
