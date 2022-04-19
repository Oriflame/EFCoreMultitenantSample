namespace EFCoreMultitenantSample.Infrastructure.TenantSupport;

public class TenantConfigurationOptions
{
    public const string ConfigKey = "TenantConfiguration";
    public class Tenant
    {
        public string Name { get; set; } = null!;

        public string? ConnectionString { get; set; }
    }

    public string ConnectionString { get; set; } = null!;

    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
