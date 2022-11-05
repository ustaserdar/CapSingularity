using System.Text.Json.Serialization;

namespace CapSingularity.Models;

public abstract class ExternalEventBase : IBaseExternalEvent
{
    private string _messageId { get; set; }

    [JsonPropertyName("messageId")]
    public virtual string MessageId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_messageId))
            {
                _messageId = Guid.NewGuid().ToString();
            }

            return _messageId;
        }
        set => _messageId = value;
    }
}