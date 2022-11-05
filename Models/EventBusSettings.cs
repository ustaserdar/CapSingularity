namespace CapSingularity.Models;

public class EventBusSettings
{
    public bool Enabled { get; set; }
    public bool UseDefaultSerializer { get; set; }
    public RabbitMqSettings RabbitMQ { get; set; }
    public CapSettings Cap { get; set; }
    public CapMongoDbSettings MongoDbSettings { get; set; }
}

public class RabbitMqSettings
{
    public List<string> ClusterMembers { get; set; }
    public string VirtualHost { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool HasCustomHeaders { get; set; }
}

public class CapSettings
{
    public string ExchangeName { get; set; }
    public string DefaultGroupName { get; set; }
    public string PublishedCollection { get; set; }
    public string ReceivedCollection { get; set; }
    public string SingularityCollection { get; set; }
    public int FailedRetryInterval { get; set; }
    public int FailedRetryCount { get; set; }
    public string DashboardPath { get; set; }
    public int SucceedMessageExpiredAfter { get; set; }
}

public class CapMongoDbSettings
{
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public CapMongoDbSettingsReplicaModel ReplicaSet { get; set; }

    public string GetConnectionString()
    {
        return $"mongodb://{UserName}:{Password}@{string.Join(',', ReplicaSet.Endpoints.Select(s => $"{s.Name}:{s.Port}").ToList())}/?authSource={DatabaseName}";
    }
}

public class CapMongoDbSettingsReplicaModel
{
    public string Name { get; set; }
    public List<CapMongoDbSettingsReplicaEndpointModel> Endpoints { get; set; }
}

public class CapMongoDbSettingsReplicaEndpointModel
{
    public string Name { get; set; }
    public int Port { get; set; }
}