using Microsoft.Extensions.Options;

namespace EFCoreMultitenantSample.Infrastructure.TenantSupport;

public class TenantProvider : ITenantProvider
{
    public const string DefaultSchemaName = "dbo";
    private readonly string _defaultConnectionString;
    private readonly Dictionary<string, string> _connectionStringPerTenant;

    public string? CurrentTenant => TenantContext.CurrentTenant;

    public string DbSchemaName => CurrentTenant ?? DefaultSchemaName;

    public string ConnectionString
    {
        get
        {
            if (CurrentTenant != null && _connectionStringPerTenant.TryGetValue(CurrentTenant, out var connectionString))
            {
                return connectionString;
            }

            return _defaultConnectionString;
        }
    }

    public TenantProvider(IOptions<TenantConfigurationOptions> tenantConfigurationOptions)
    {
        _defaultConnectionString = tenantConfigurationOptions.Value.ConnectionString;

        _connectionStringPerTenant = tenantConfigurationOptions.Value.Tenants
            .Where(t => !string.IsNullOrWhiteSpace(t.ConnectionString))
            .ToDictionary(ks => ks.Name, vs => vs.ConnectionString!);
    }
    
    public IDisposable BeginScope(string tenant)
    {
        return TenantContext.BeginScope(tenant);
    }

    public override string? ToString()
    {
        return CurrentTenant;
    }
}
