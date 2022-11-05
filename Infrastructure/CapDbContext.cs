using System.Diagnostics.CodeAnalysis;
using CapSingularity.Models;
using MongoDB.Driver;

namespace CapSingularity.Infrastructure;

[ExcludeFromCodeCoverage]
public class CapDbContext
{
    private readonly IMongoDatabase _db;
    private readonly IMongoClient _client;
    private readonly string _singularityCollectionName;
    public CapDbContext(CapSettings settings, CapMongoDbSettings capMongoDbSettings)
    {
        _client = new MongoClient(capMongoDbSettings.GetConnectionString());
        _db = _client.GetDatabase(capMongoDbSettings.DatabaseName);
        _singularityCollectionName = settings.SingularityCollection;
    }
    public IMongoCollection<T> GetCollection<T>() => _db.GetCollection<T>(_singularityCollectionName);
    public IMongoClient Client => _client;
}