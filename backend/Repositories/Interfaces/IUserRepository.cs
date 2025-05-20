using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);

    
    


}