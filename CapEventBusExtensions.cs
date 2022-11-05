using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CapSingularity.Clients;
using CapSingularity.Infrastructure;
using CapSingularity.Models;
using DotNetCore.CAP;
using DotNetCore.CAP.Filter;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Serialization;

namespace CapSingularity;

public static class CapEventBusExtensions
{
    public static IServiceCollection AddEventBusRegistration<T>(this IServiceCollection services,
                                                                IConfiguration configuration) where T : class, ISubscribeFilter
    {
        var eventBusSettings = configuration.GetSection("EventBusSettings").Get<EventBusSettings>();
        services.Configure<EventBusSettings>(configuration.GetSection("EventBusSettings"));

        if (eventBusSettings is not { Enabled: true })
            return services;

        if (eventBusSettings is { UseDefaultSerializer: false })
            services.AddSingleton<ISerializer, CapNewtonsoftSerializer>();

        services.InitializeCapBuilder(eventBusSettings).AddSubscribeFilter<T>();
        services.AddScoped<IEventBusClient, CapEventBusClient>();

        services.AddDataRegistration(eventBusSettings.Cap, eventBusSettings.MongoDbSettings);
        return services;
    }

    private static CapBuilder InitializeCapBuilder(this IServiceCollection services,
                                                   EventBusSettings eventBusSettings,
                                                   Action<FailedInfo> failedTresholdCallback = null)
    {
        var builder = services.AddCap(x =>
        {
            x.UseMongoDB(cfg =>
            {
                cfg.DatabaseConnection = eventBusSettings.MongoDbSettings.GetConnectionString();
                cfg.DatabaseName = eventBusSettings.MongoDbSettings.DatabaseName;
                cfg.PublishedCollection = eventBusSettings.Cap.PublishedCollection;
                cfg.ReceivedCollection = eventBusSettings.Cap.ReceivedCollection;
            });

            x.UseRabbitMQ(cfg =>
            {
                cfg.HostName = string.Join(',', eventBusSettings.RabbitMQ.ClusterMembers);
                cfg.Port = eventBusSettings.RabbitMQ.Port;
                cfg.VirtualHost = eventBusSettings.RabbitMQ.VirtualHost;
                cfg.UserName = eventBusSettings.RabbitMQ.Username;
                cfg.Password = eventBusSettings.RabbitMQ.Password;
                cfg.ExchangeName = eventBusSettings.Cap.ExchangeName;

                if (eventBusSettings.RabbitMQ.HasCustomHeaders)
                {
                    cfg.CustomHeaders = e => new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(DotNetCore.CAP.Messages.Headers.MessageId,
                            SnowflakeId.Default().NextId().ToString()),
                        new KeyValuePair<string, string>(DotNetCore.CAP.Messages.Headers.MessageName, e.RoutingKey)
                    };
                }
            });

            x.DefaultGroupName = eventBusSettings.Cap.DefaultGroupName;
            if (eventBusSettings.Cap.SucceedMessageExpiredAfter != 0)
                x.SucceedMessageExpiredAfter = eventBusSettings.Cap.SucceedMessageExpiredAfter;
            x.FailedRetryInterval = eventBusSettings.Cap.FailedRetryInterval;
            x.FailedRetryCount = eventBusSettings.Cap.FailedRetryCount;
            x.FailedThresholdCallback = failedTresholdCallback;
            x.UseDashboard(p => p.PathMatch = eventBusSettings.Cap.DashboardPath);

            if (eventBusSettings is { UseDefaultSerializer: true })
            {
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            }
        });

        return builder;
    }

    private static IServiceCollection AddDataRegistration(this IServiceCollection services,
                                                          CapSettings capSettings,
                                                          CapMongoDbSettings capMongoDbSettings)
    {
        services.AddSingleton(capSettings);
        services.AddSingleton(capMongoDbSettings);
        services.AddScoped<CapDbContext>();
        services.AddScoped<CapSingularityRepository>();
        return services;
    }
}