using EFCoreMultitenantSample.Infrastructure.TenantSupport;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EFCoreMultitenantSample.Persistence.EF.SQL.Infrastructure;

internal class DbSchemaAwareSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
{
    private readonly ITenantProvider _tenantProvider;

    public DbSchemaAwareSqlServerMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IRelationalAnnotationProvider migrationsAnnotations,
        ITenantProvider tenantProvider)
        : base(dependencies, migrationsAnnotations)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        ChangeSchema(operation);

        base.Generate(operation, model, builder);
    }

    private void ChangeSchema(MigrationOperation? operation)
    {
        if (operation == null)
        {
            return;
        }

        switch (operation)
        {
            case SqlServerCreateDatabaseOperation _:
            case SqlServerDropDatabaseOperation _:
                break;
            case EnsureSchemaOperation ensureSchemaOperation:
                ensureSchemaOperation.Name = _tenantProvider.DbSchemaName;
                break;
            case CreateTableOperation createTableOperation:
                createTableOperation.Schema = _tenantProvider.DbSchemaName;
                foreach (var foreignKey in createTableOperation.ForeignKeys)
                {
                    ChangeSchema(foreignKey);
                }
                break;
            case DropTableOperation dropTableOperation:
                dropTableOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case CreateIndexOperation createIndexOperation:
                createIndexOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case AddColumnOperation addColumnOperation:
                addColumnOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case AlterColumnOperation alterColumnOperation:
                alterColumnOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case DropColumnOperation dropColumnOperation:
                dropColumnOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case RenameColumnOperation renameColumnOperation:
                renameColumnOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case AddForeignKeyOperation addForeignKeyOperation:
                addForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                addForeignKeyOperation.PrincipalSchema = _tenantProvider.DbSchemaName;
                break;
            case DropForeignKeyOperation dropForeignKeyOperation:
                dropForeignKeyOperation.Schema = _tenantProvider.DbSchemaName;
                break;
            case RenameTableOperation renameTableOperation:
                renameTableOperation.Schema = _tenantProvider.DbSchemaName;
                renameTableOperation.NewSchema = _tenantProvider.DbSchemaName;
                break;
            default:
                throw new NotImplementedException(
                    $"Migration operation of type {operation.GetType().Name} is not supported by DbSchemaAwareSqlServerMigrationsSqlGenerator.");
        }
    }
}
