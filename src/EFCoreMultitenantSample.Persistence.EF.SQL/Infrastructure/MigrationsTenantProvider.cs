using EFCoreMultitenantSample.Infrastructure.TenantSupport;

namespace EFCoreMultitenantSample.Persistence.EF.SQL.Infrastructure;

public class MigrationsTenantProvider : ITenantProvider
{
    public string? CurrentTenant => null;

    public string DbSchemaName => "dbo";

    public string ConnectionString => "Persist Security Info=True;Integrated Security=true;Server=.;Database=MultiTenantSample;";

    public IDisposable BeginScope(string tenant)
    {
        throw new NotImplementedException();
    }
}
