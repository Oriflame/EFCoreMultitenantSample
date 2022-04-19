using EFCoreMultitenantSample.Infrastructure.TenantSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCoreMultitenantSample.Persistence.EF.SQL.Infrastructure;

internal class DbSchemaAwareModelCacheKeyFactory : IModelCacheKeyFactory
{
    private readonly ITenantProvider _tenantProvider;

    public DbSchemaAwareModelCacheKeyFactory(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public object Create(DbContext context, bool designTime)
    {
        return Tuple.Create(context.GetType(), _tenantProvider.DbSchemaName, designTime);
    }
}
