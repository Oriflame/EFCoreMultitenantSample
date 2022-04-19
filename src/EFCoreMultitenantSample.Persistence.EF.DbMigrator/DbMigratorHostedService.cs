using EFCoreMultitenantSample.Domain;
using EFCoreMultitenantSample.Infrastructure.TenantSupport;
using EFCoreMultitenantSample.Persistence.EF.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EFCoreMultitenantSample.Persistence.EF.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly ILogger<DbMigratorHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantProvider _tenantProvider;
    private readonly IOptions<TenantConfigurationOptions> _tenantConfigurationOptions;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(ILogger<DbMigratorHostedService> logger, IServiceProvider serviceProvider, ITenantProvider tenantProvider,
        IOptions<TenantConfigurationOptions> tenantConfigurationOptions, IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _tenantProvider = tenantProvider;
        _tenantConfigurationOptions = tenantConfigurationOptions;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await MigateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error when migrating database");
        }

        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task MigateAsync()
    {
        var supportedTenants = _tenantConfigurationOptions.Value.Tenants.Select(t => t.Name);
        foreach (var tenant in supportedTenants)
        {
            using var loggerScope = _logger.BeginScope("tenant={tenant}", tenant);
            _logger.LogInformation("Migrating database for tenant {tenant}", tenant);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                using var tenantContext = _tenantProvider.BeginScope(tenant);
                var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>();

                await dbContext.Database.MigrateAsync();

                _logger.LogInformation("Database was migrated for tenant {tenant}", tenant);

                await SeedCustomer(dbContext, tenant);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error when migrating database for tenant {tenant}", tenant);
            }
        }
    }

    private async Task SeedCustomer(SampleDbContext dbContext, string tenant)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync();
        if (customer == null)
        {
            var tenantPrefix = tenant.ToUpper() + " ";
            dbContext.Customers.Add(new Customer
            {
                FirstName = tenantPrefix + "John",
                LastName = tenantPrefix + "Doe"
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
