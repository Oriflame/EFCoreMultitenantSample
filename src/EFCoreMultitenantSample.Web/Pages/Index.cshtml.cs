using EFCoreMultitenantSample.Domain;
using EFCoreMultitenantSample.Infrastructure.TenantSupport;
using EFCoreMultitenantSample.Persistence.EF.SQL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EFCoreMultitenantSample.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IOptions<TenantConfigurationOptions> _tenantConfigurationOptions;
    private readonly ITenantProvider _tenantProvider;
    private readonly SampleDbContext _dbContext;

    public IEnumerable<string> Tenants => _tenantConfigurationOptions.Value.Tenants.Select(t => t.Name);

    public string? CurrentTenant => _tenantProvider.CurrentTenant;

    public Customer? CustomerNumberOne { get; private set; }

    public IndexModel(ILogger<IndexModel> logger, IOptions<TenantConfigurationOptions> tenantConfigurationOptions, ITenantProvider tenantProvider,
        SampleDbContext dbContext)
    {
        _logger = logger;
        _tenantConfigurationOptions = tenantConfigurationOptions;
        _tenantProvider = tenantProvider;
        _dbContext = dbContext;
    }

    public async Task OnGet()
    {
        if (CurrentTenant != null)
        {
            CustomerNumberOne = await _dbContext.Customers.FirstOrDefaultAsync();
        }
    }
}
