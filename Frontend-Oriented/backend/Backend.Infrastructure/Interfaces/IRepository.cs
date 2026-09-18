namespace Backend.Infrastructure.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);
}
