using CapSingularity.Exceptions;
using CapSingularity.Infrastructure;
using CapSingularity.Models;
using DotNetCore.CAP.Filter;
using MongoDB.Driver;

namespace CapSingularity.Filters;

public class CapSubscribeFilter : SubscribeFilter
{
    private readonly CapSingularityRepository _singularityRepo;

    public CapSubscribeFilter(CapSingularityRepository singularityRepo)
    {
        _singularityRepo = singularityRepo;
    }
    
    public override void OnSubscribeException(ExceptionContext context)
    {
        if (context.Exception is CapSingularityException)
        {
            context.ExceptionHandled = true;
            return;
        }

        var messageId = (context.DeliverMessage.Value as ExternalEventBase).MessageId;
        var result = _singularityRepo.DeleteByIdAsync(messageId).Result;
    }
    
    public override void OnSubscribeExecuting(ExecutingContext context)
    {
        var messageId = (context.DeliverMessage.Value as ExternalEventBase).MessageId;
        if (!CanOperateCapMessage(messageId))
        {
            throw new CapSingularityException();
        }
    }

    private bool CanOperateCapMessage(string messageId)
    {
        try
        {
            var singularityRecord = new CapSingularityModel(messageId, DateTime.UtcNow);
            return _singularityRepo.InsertAsync(singularityRecord).Result;
        }
        catch (AggregateException e)
        {
            if (e.InnerException is MongoWriteException &&
                ((MongoWriteException)e.InnerException).WriteError.Category == ServerErrorCategory.DuplicateKey)
                return false;
            throw;
        }
    }
}