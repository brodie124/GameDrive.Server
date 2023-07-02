using System.Linq.Expressions;

namespace GameDrive.Server.Services.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);
    Task SaveChangesAsync();
}