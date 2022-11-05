using CapSingularity.Models;

namespace CapSingularity.Clients;

public interface IEventBusClient
{
    Task PublishAsync<T>(T message) where T : ExternalEventBase;
    Task PublishAsync<T>(T message, IDictionary<string, string> headers) where T : ExternalEventBase;
    Task PublishAsync<T>(T message, string eventKey) where T : ExternalEventBase;
    Task PublishAsync<T>(T message, string eventKey, IDictionary<string, string> headers) where T : ExternalEventBase;
}
