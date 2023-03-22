using DatabasePart.Helpers.DependencyInjection;
using EntityConfiguration.Configuration;
using EntityConfiguration.MySql;
using EntityConfiguration.PostgreSQL;
using EntityConfiguration.SqlServer;
using EntityScheme;
using EntityShare.DbContexts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EntityScheme.Helpers;

namespace DatabasePart.Helpers
{
    public static class StartupHelpers
    {
        public static void AddMvcExceptionFilters(this IServiceCollection services)
        {
            //Exception handling
            //services.AddScoped<ControllerExceptionFilterAttribute>();
        }

        public static IServiceCollection AddMigration<TDbContext, TDataProtectionDbContext>(this IServiceCollection services,
            Action<IdentityServer4AdminUIOptions> optionsAction)
            where TDbContext: DbContext,IDbContext
            where TDataProtectionDbContext: DbContext, IDataProtectionKeyContext
        {
            var options = new IdentityServer4AdminUIOptions();
            optionsAction(options);

            services.AddSingleton(options.TestData);

            services.RegisterDbContexts<TDbContext, TDataProtectionDbContext>(options.ConnectionStrings, options.DatabaseProvider, options.DatabaseMigrations);

            services.AddDataProtection()
                .SetApplicationName("IS4R")
                .PersistKeysToDbContext<TDataProtectionDbContext>();

            // Add health checks.
            var healthChecksBuilder = options.HealthChecksBuilderFactory?.Invoke(services) ?? services.AddHealthChecks();
            healthChecksBuilder.AddIdSHealthChecks<TDbContext,TDataProtectionDbContext>
                (options.ConnectionStrings, options.DatabaseProvider);

            return services;
        }

        public static void AddIdSHealthChecks<TDbContext,TDataProtectionDbContext>
            (this IHealthChecksBuilder healthChecksBuilder,
            ConnectionStringsConfiguration connectionStringsConfiguration, DatabaseProviderConfiguration databaseProviderConfiguration)
            where TDbContext : DbContext, IDbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var configurationDbConnectionString = connectionStringsConfiguration.DbConnection;
            var dataProtectionDbConnectionString = connectionStringsConfiguration.DataProtectionDbConnection;

            healthChecksBuilder = healthChecksBuilder
                .AddDbContextCheck<TDbContext>("ConfigurationDbContext")
                .AddDbContextCheck<TDataProtectionDbContext>("DataProtectionDbContext");

            var serviceProvider = healthChecksBuilder.Services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var configurationTableName = DbContextHelpers.GetEntityTable<TDbContext>(scope.ServiceProvider);
                var dataProtectionTableName = DbContextHelpers.GetEntityTable<TDataProtectionDbContext>(scope.ServiceProvider);

                switch (databaseProviderConfiguration.ProviderType)
                {
                    case DatabaseProviderType.SqlServer:
                        healthChecksBuilder
                            .AddSqlServer(configurationDbConnectionString, name: "ConfigurationDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{configurationTableName}]")
                            .AddSqlServer(dataProtectionDbConnectionString, name: "DataProtectionDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataProtectionTableName}]");
                        break;
                    case DatabaseProviderType.PostgreSQL:
                        healthChecksBuilder
                           .AddNpgSql(configurationDbConnectionString, name: "ConfigurationDb",
                               healthQuery: $"SELECT * FROM \"{configurationTableName}\" LIMIT 1")
                           .AddNpgSql(dataProtectionDbConnectionString, name: "DataProtectionDb",
                               healthQuery: $"SELECT * FROM \"{dataProtectionTableName}\"  LIMIT 1");
                        break;
                    case DatabaseProviderType.MySql:
                        healthChecksBuilder
                           .AddMySql(configurationDbConnectionString, name: "ConfigurationDb")
                           .AddMySql(dataProtectionDbConnectionString, name: "DataProtectionDb");
                        break;
                    default:
                        throw new NotImplementedException($"Health checks not defined for database provider {databaseProviderConfiguration.ProviderType}");
                }
            }
        }

        public static void RegisterDbContexts<TDbContext, TDataProtectionDbContext>(
           this IServiceCollection services,
           ConnectionStringsConfiguration connectionStrings,
           DatabaseProviderConfiguration databaseProvider,
           DatabaseMigrationsConfiguration databaseMigrations)
           where TDbContext : DbContext, IDbContext
           where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {


            switch (databaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    services.RegisterSqlServerDbContexts<TDbContext, TDataProtectionDbContext>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.PostgreSQL:
                    services.RegisterNpgSqlDbContexts<TDbContext, TDataProtectionDbContext>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.MySql:
                    services.RegisterMySqlDbContexts<TDbContext, TDataProtectionDbContext>(connectionStrings, databaseMigrations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType), $@"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
            }
        }
    }
}
