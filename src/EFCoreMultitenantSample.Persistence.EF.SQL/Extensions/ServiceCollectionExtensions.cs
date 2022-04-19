using EFCoreMultitenantSample.Infrastructure.TenantSupport;
using EFCoreMultitenantSample.Persistence.EF.SQL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EFCoreMultitenantSample.Persistence.EF.SQL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkSqlServer<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddSingleton<IModelCacheKeyFactory, DbSchemaAwareModelCacheKeyFactory>();
        services.AddScoped<IMigrationsSqlGenerator, DbSchemaAwareSqlServerMigrationsSqlGenerator>();

        services.TryAddSingleton<ITenantProvider, TenantProvider>();

        services
            .AddEntityFrameworkSqlServer()
            .AddDbContext<TContext>((sp, options) =>
                options.UseInternalServiceProvider(sp)
            );

        return services;
    }
}
