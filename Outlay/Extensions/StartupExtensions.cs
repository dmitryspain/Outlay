using Microsoft.EntityFrameworkCore;
using Outlay.Infrastructure.InMemoryDb;
using Outlay.Infrastructure.Mapper;
using Outlay.Models;
using StackExchange.Redis;

namespace Outlay.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddInMemoryDbContext(this IServiceCollection services)
    {
        services.AddDbContext<OutlayInMemoryContext>(
            options => options.UseInMemoryDatabase(databaseName: "OutlayInMemoryContext"), ServiceLifetime.Singleton);
        var context = services.BuildServiceProvider().GetRequiredService<OutlayInMemoryContext>();
        MccInfoInitializer.AddMccs(context, CancellationToken.None).Wait();
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = configuration.GetSection(SectionConstants.Redis).Value;
            option.InstanceName = "master";
        });

        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(TransactionProfile).Assembly);
        return services;
    }
}