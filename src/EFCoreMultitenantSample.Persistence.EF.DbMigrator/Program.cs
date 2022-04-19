using System.Reflection;
using EFCoreMultitenantSample.Infrastructure.Extensions;
using EFCoreMultitenantSample.Persistence.EF.DbMigrator;
using EFCoreMultitenantSample.Persistence.EF.SQL;
using EFCoreMultitenantSample.Persistence.EF.SQL.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        // need to set base path to make it work with shared appsettings files
        configBuilder.SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTenantSupport(hostContext.Configuration);
        services.AddEntityFrameworkSqlServer<SampleDbContext>();
        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();
