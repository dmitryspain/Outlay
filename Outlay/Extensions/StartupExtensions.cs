using Microsoft.EntityFrameworkCore;
using Outlay.Db;

namespace Outlay.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddInMemoryDbContext(this IServiceCollection services)
    {
        services.AddDbContext<OutlayInMemoryContext>(
            options => options.UseInMemoryDatabase(databaseName: "OutlayInMemoryContext"), ServiceLifetime.Singleton);
        var context = services.BuildServiceProvider().GetRequiredService<OutlayInMemoryContext>();
        MccInfoInitializer.AddMccs(context);
        return services;
    }
}