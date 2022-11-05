using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Serialization;
using Newtonsoft.Json;
using System.Text.Json;

namespace CapSingularity;
public class CapNewtonsoftSerializer : ISerializer
{
    public Task<TransportMessage> SerializeAsync(Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (message.Value == null)
            return Task.FromResult(new TransportMessage(message.Headers, null));

        var jsonString = JsonConvert.SerializeObject(message.Value);
        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        return Task.FromResult(new TransportMessage(message.Headers, jsonBytes));
    }

    public Task<Message> DeserializeAsync(TransportMessage transportMessage, Type valueType)
    {
        if (valueType == null || transportMessage.Body == null)
            return Task.FromResult(new Message(transportMessage.Headers, null));

        var jsonString = System.Text.Encoding.UTF8.GetString(transportMessage.Body);
        var obj = JsonConvert.DeserializeObject(jsonString, valueType);

        return Task.FromResult(new Message(transportMessage.Headers, obj));
    }

    public string Serialize(Message message) => JsonConvert.SerializeObject(message);

    public Message Deserialize(string json) => JsonConvert.DeserializeObject<Message>(json);

    public object Deserialize(object value, Type valueType) => JsonConvert.DeserializeObject((string)value, valueType);

    public bool IsJsonType(object jsonObject) => jsonObject is JsonElement;
}