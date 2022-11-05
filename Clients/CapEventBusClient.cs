using CapSingularity.Models;
using DotNetCore.CAP;

namespace CapSingularity.Clients;

public class CapEventBusClient : IEventBusClient
{
    private readonly ICapPublisher _capPublisher;

    public CapEventBusClient(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
    }

    public async Task PublishAsync<T>(T message) where T : ExternalEventBase
    {
        await _capPublisher.PublishAsync($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower()}.{typeof(T).Name}", message);
    }

    public async Task PublishAsync<T>(T message, IDictionary<string, string> headers) where T : ExternalEventBase
    {
        await _capPublisher.PublishAsync($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower()}.{typeof(T).Name}", message, headers);
    }

    public async Task PublishAsync<T>(T message, string routingKey) where T : ExternalEventBase
    {
        await _capPublisher.PublishAsync(routingKey, message);    
    }

    public async Task PublishAsync<T>(T message, string eventKey, IDictionary<string, string> headers) where T : ExternalEventBase
    {
        await _capPublisher.PublishAsync(eventKey, message, headers);
    }
}