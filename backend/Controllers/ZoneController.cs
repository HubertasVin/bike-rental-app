using BikeRentalApp.Application.DTOs;
using BikeRentalApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeRentalApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        /// <summary>
        /// Gets all zones.
        /// </summary>
        /// <returns>A list of all zones.</returns>
        /// <response code="200">Returns the list of zones.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ZoneDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _zoneService.GetAllZonesAsync();
            return Ok(zones);
        }

        /// <summary>
        /// Gets all zones with their bike counts.
        /// </summary>
        /// <returns>A list of all zones with bike counts.</returns>
        /// <response code="200">Returns the list of zones with bike counts.</response>
        [HttpGet("bikes_count")]
        [ProducesResponseType(typeof(IEnumerable<ZoneWithBikesNumberDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWithBikeCounts()
        {
            var zones = await _zoneService.GetAllZonesWithBikesCountAsync();
            return Ok(zones);
        }

        /// <summary>
        /// Gets a specific zone by its unique ID.
        /// </summary>
        /// <param name="id">The GUID of the zone to retrieve.</param>
        /// <returns>The requested zone.</returns>
        /// <response code="200">Returns the zone.</response>
        /// <response code="404">If the zone is not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ZoneDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null) return NotFound();
            return Ok(zone);
        }

        /// <summary>
        /// Creates a new zone.
        /// </summary>
        /// <param name="dto">The details of the zone to create.</param>
        /// <returns>The newly created zone.</returns>
        /// <response code="201">Returns the newly created zone.</response>
        /// <response code="400">If the input is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ZoneDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateZoneDTO dto)
        {
            var created = await _zoneService.CreateZoneAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing zone.
        /// </summary>
        /// <param name="id">The GUID of the zone to update.</param>
        /// <param name="dto">The updated zone details.</param>
        /// <returns>The updated zone.</returns>
        /// <response code="200">Returns the updated zone.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="404">If the zone does not exist.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ZoneDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateZoneDTO dto)
        {
            try
            {
                var updated = await _zoneService.UpdateZoneAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a zone by its ID.
        /// </summary>
        /// <param name="id">The GUID of the zone to delete.</param>
        /// <response code="204">Zone was successfully deleted.</response>
        /// <response code="404">If the zone was not found.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _zoneService.DeleteZoneAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
