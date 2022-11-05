
namespace CapSingularity.Infrastructure;

public class CapSingularityModel
{
    public CapSingularityModel(string messageId, DateTime createdAt)
    {
        CapMessageId = messageId;
        CreatedAt = createdAt;
    }
    public string CapMessageId { get; set; }
    public DateTime CreatedAt { get; set; }
}