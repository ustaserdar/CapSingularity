using MongoDB.Driver;

namespace CapSingularity.Infrastructure;

public class CapSingularityRepository : IRepository<CapSingularityModel>
{
    private readonly CapDbContext _capDbContext;
    private readonly IMongoCollection<CapSingularityModel> _collection;
    public CapSingularityRepository(CapDbContext capDbContext)
    {
        _capDbContext = capDbContext;
        _collection = capDbContext.GetCollection<CapSingularityModel>();
    }

    public async Task<CapSingularityModel> GetByIdAsync(string id)
    {
        var result = await _collection.FindAsync(e => e.CapMessageId == id);
        return await result.FirstOrDefaultAsync();
    }

    public virtual async Task<bool> InsertAsync(CapSingularityModel entity)
    {
        await _collection.InsertOneAsync(entity);
        return true;
    }

    public virtual async Task<bool> DeleteByIdAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(q => q.CapMessageId.Equals(id));
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}