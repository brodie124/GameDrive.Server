using System.Linq.Expressions;
using GameDrive.Server.Database;
using Microsoft.EntityFrameworkCore;

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
}

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly GameDriveDbContext _dbContext;

    public GenericRepository(GameDriveDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public async Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await GetBaseQueryable().Where(expression).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await GetBaseQueryable().ToListAsync();
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbContext.Update(entity);
        await SaveChangesAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
    
    protected virtual IQueryable<T> GetBaseQueryable()
    {
        return _dbContext
            .Set<T>()
            .AsQueryable();
    }
}