using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace BikeRentalApp.Repositories.Decorators;

public class LoggingRentalRepositoryDecorator : IRentalRepositoryDecorator
{
    private readonly IRentalRepository _decoratedRepository;
    private readonly ILogger<LoggingRentalRepositoryDecorator> _logger;

    public LoggingRentalRepositoryDecorator(
        IRentalRepository decoratedRepository,
        ILogger<LoggingRentalRepositoryDecorator> logger)
    {
        _decoratedRepository = decoratedRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        _logger.LogInformation("Getting all rentals");
        return await _decoratedRepository.GetAllAsync();
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting rental by id: {RentalId}", id);
        return await _decoratedRepository.GetByIdAsync(id);
    }

    public async Task<Rental> CreateAsync(Rental rental)
    {
        _logger.LogInformation("Creating rental for user: {UserId}, bike: {BikeId}", rental.UserId, rental.BikeId);
        return await _decoratedRepository.CreateAsync(rental);
    }

    public async Task<Rental?> UpdateAsync(Rental rental)
    {
        _logger.LogInformation("Updating rental: {RentalId}", rental.Id);
        return await _decoratedRepository.UpdateAsync(rental);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting rental: {RentalId}", id);
        return await _decoratedRepository.DeleteAsync(id);
    }

    public async Task<Rental?> GetActiveRentalForUserAsync(Guid userId)
    {
        _logger.LogInformation("Getting active rental for user: {UserId}", userId);
        return await _decoratedRepository.GetActiveRentalForUserAsync(userId);
    }

    public async Task<bool> HasActiveRentalForBikeAsync(Guid bikeId)
    {
        _logger.LogInformation("Checking active rental for bike: {BikeId}", bikeId);
        return await _decoratedRepository.HasActiveRentalForBikeAsync(bikeId);
    }
}
