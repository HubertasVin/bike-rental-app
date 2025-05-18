using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeRentalApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RentalController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly IReservationService _reservationService;

    public RentalController(IRentalService rentalService, IReservationService reservationService)
    {
        _rentalService = rentalService;
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
    /// Creates a rental from an existing reservation (unlocks the bike)
    /// </summary>
    /// <param name="reservationId">Reservation ID</param>
    /// <response code="201">Created rental</response>
    /// <response code="400">If rental cannot be created from reservation</response>
    [HttpPost("reservation/{reservationId:guid}")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFromReservation(Guid reservationId)
    {
        var userId = GetCurrentUserId();
        var rental = await _rentalService.CreateRentalFromReservationAsync(userId, reservationId);

        if (rental == null)
        {
            return BadRequest(
                "Unable to create rental from reservation. Check if the reservation exists and is active."
            );
        }

        return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
    }

    /// <summary>
    /// Gets all rentals
    /// </summary>
    /// <response code="200">List of all rentals</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RentalDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        return Ok(rentals);
    }

    /// <summary>
    /// Gets a rental by ID
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <response code="200">Rental details</response>
    /// <response code="404">If rental not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);

        if (rental == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        if (rental.UserId != userId)
        {
            return Forbid();
        }

        return Ok(rental);
    }

    /// <summary>
    /// Gets current user's active rental
    /// </summary>
    /// <response code="200">Active rental or null if none exists</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        var userId = GetCurrentUserId();
        var rental = await _rentalService.GetActiveRentalForUserAsync(userId);
        return Ok(rental);
    }

    /// <summary>
    /// Ends a rental (returns the bike)
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <param name="zoneId">Zone ID where the bike is being returned</param>
    /// <response code="200">Rental ended successfully with cost information</response>
    /// <response code="400">If zone is not allowed for return</response>
    /// <response code="404">If rental not found</response>
    [HttpPost("{id:guid}/end")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndRental(Guid id, [FromQuery] Guid zoneId)
    {
        var userId = GetCurrentUserId();

        var isZoneAllowed = await _rentalService.IsInAllowedZoneAsync(id, zoneId);
        if (!isZoneAllowed)
        {
            return BadRequest("The bike cannot be returned in this zone.");
        }

        var ended = await _rentalService.EndRentalAsync(id, userId, zoneId);
        if (!ended)
        {
            return NotFound();
        }

        var cost = await _rentalService.CalculateRentalCostAsync(id);
        return Ok(cost);
    }

    /// <summary>
    /// Deletes a rental
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <response code="204">No content</response>
    /// <response code="404">If rental not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _rentalService.DeleteRentalAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Locks the bike during an active rental
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <response code="200">Bike locked successfully</response>
    /// <response code="404">If rental not found</response>
    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LockBike(Guid id)
    {
        var userId = GetCurrentUserId();
        var rental = await _rentalService.LockBikeAsync(id, userId);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    /// <summary>
    /// Unlocks the bike during an active rental
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <response code="200">Bike unlocked successfully</response>
    /// <response code="404">If rental not found</response>
    [HttpPost("{id:guid}/unlock")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlockBike(Guid id)
    {
        var userId = GetCurrentUserId();
        var rental = await _rentalService.UnlockBikeAsync(id, userId);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    /// <summary>
    /// Calculates the current cost of an active rental
    /// </summary>
    /// <param name="id">Rental ID</param>
    /// <response code="200">Current cost of rental</response>
    /// <response code="404">If rental not found</response>
    [HttpGet("{id:guid}/cost")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateCost(Guid id)
    {
        var cost = await _rentalService.CalculateRentalCostAsync(id);

        if (cost == null)
        {
            return NotFound();
        }

        return Ok(cost);
    }
}
