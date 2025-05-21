using BikeRentalApp.Data;
using BikeRentalApp.Domain.Entities;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public new async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        await AddAsync(user);
        await SaveChangesAsync();
        return user;
    }


    public new async Task<User?> UpdateAsync(User user)
    {
        var existingUser = await GetByIdAsync(user.Id);
        if (existingUser == null)
        {
            return null;
        }

        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

}