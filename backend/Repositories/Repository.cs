using System.Linq.Expressions;
using BikeRentalApp.Data;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeRentalApp.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetAll() => _dbSet;

    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => 
        await _dbSet.Where(predicate).ToListAsync();

    public async Task SaveChangesAsync() => 
        await _context.SaveChangesAsync();
}
