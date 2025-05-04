using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeRentalApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BikeController : ControllerBase
{
    private readonly IBikeService _bikeService;

    public BikeController(IBikeService bikeService)
    {
        _bikeService = bikeService;
    }

    /// <summary>
    /// Gets all bikes
    /// </summary>
    /// <returns>List of all bikes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BikeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var bikes = await _bikeService.GetAllBikesAsync();
        return Ok(bikes);
    }

    /// <summary>
    /// Gets a bike by ID
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <returns>Bike details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BikeDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var bike = await _bikeService.GetBikeByIdAsync(id);
        if (bike == null)
        {
            return NotFound();
        }

        return Ok(bike);
    }

    /// <summary>
    /// Creates a new bike
    /// </summary>
    /// <param name="createBikeDto">New bike details</param>
    /// <returns>Created bike</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BikeDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateBikeDTO createBikeDto)
    {
        var createdBike = await _bikeService.CreateBikeAsync(createBikeDto);
        return CreatedAtAction(nameof(GetById), new { id = createdBike.Id }, createdBike);
    }

    /// <summary>
    /// Updates an existing bike
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <param name="updateBikeDto">Updated bike details</param>
    /// <returns>Updated bike</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BikeDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateBikeDTO updateBikeDto)
    {
        try
        {
            var updatedBike = await _bikeService.UpdateBikeAsync(id, updateBikeDto);
            if (updatedBike == null)
            {
                return NotFound();
            }

            return Ok(updatedBike);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a bike
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bikeService.DeleteBikeAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all available bikes
    /// </summary>
    /// <returns>List of available bikes</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<BikeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable()
    {
        var bikes = await _bikeService.GetAvailableBikesAsync();
        return Ok(bikes);
    }

    /// <summary>
    /// Gets all bikes in a specific zone
    /// </summary>
    /// <param name="zoneId">Zone ID</param>
    /// <returns>List of bikes in the zone</returns>
    [HttpGet("zone/{zoneId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<BikeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByZone(Guid zoneId)
    {
        var bikes = await _bikeService.GetBikesByZoneAsync(zoneId);
        return Ok(bikes);
    }
}
