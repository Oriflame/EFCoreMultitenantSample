using EFCoreMultitenantSample.Persistence.EF.SQL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EFCoreMultitenantSample.Persistence.EF.SQL;

public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
{
    public SampleDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SampleDbContext>();

        return new SampleDbContext(builder.Options, new MigrationsTenantProvider());
    }
}
