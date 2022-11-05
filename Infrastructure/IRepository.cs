namespace CapSingularity.Infrastructure;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(string id);
    Task<bool> InsertAsync(T entity);
    Task<bool> DeleteByIdAsync(string id);
}