using EntityConfiguration.Configuration;
using System;
using System.Reflection;
using SqlMigrationAssembly = EntitySqlServer.Helpers.MigrationAssembly;
using MySqlMigrationAssembly = EntityMySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = EntityPostgreSQL.Helpers.MigrationAssembly;

namespace DatabasePart.Configuration.Database
{
    public class MigrationAssemblyConfiguration
    {
        public static string GetMigrationAssemblyByProvider(DatabaseProviderConfiguration databaseProvider)
        {
            return databaseProvider.ProviderType switch
            {
                DatabaseProviderType.SqlServer => typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
                DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo()
                    .Assembly.GetName()
                    .Name,
                DatabaseProviderType.MySql => typeof(MySqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
