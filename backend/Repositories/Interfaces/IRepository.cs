using System.Linq.Expressions;

namespace BikeRentalApp.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
}
