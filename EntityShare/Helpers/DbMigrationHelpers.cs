using EntityConfiguration.Configuration;
using EntityScheme;
using EntityScheme.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityShare.Helpers
{
    public static class DbMigrationHelpers
    {
        public static async Task<bool> ApplyDbMigrationsWithDataSeedAsync<TDbContext, TDataProtectionDbContext>
    (IHost host, bool applyDbMigrationWithDataSeedFromProgramArguments, SeedConfiguration seedConfiguration,
            DatabaseMigrationsConfiguration databaseMigrationsConfiguration)
    where TDbContext : DbContext, IDbContext
    where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            bool migrationComplete = false;
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                if ((databaseMigrationsConfiguration != null && databaseMigrationsConfiguration.ApplyDatabaseMigrations)
                    || (applyDbMigrationWithDataSeedFromProgramArguments))
                {
                    migrationComplete = await EnsureDatabasesMigratedAsync<TDbContext, TDataProtectionDbContext>(services);
                }

                if ((seedConfiguration != null && seedConfiguration.ApplySeed)
                    || (applyDbMigrationWithDataSeedFromProgramArguments))
                {
                    var seedComplete = await EnsureSeedDataAsync<TDbContext>(services);
                    return migrationComplete && seedComplete;
                }

            }
            return migrationComplete;
        }

        public static async Task<bool> EnsureDatabasesMigratedAsync<TDbContext, TDataProtectionDbContext>(IServiceProvider services)
           where TDbContext : DbContext
           where TDataProtectionDbContext : DbContext
        {
            int pendingMigrationCount = 0;
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TDbContext>())
                {
                    await context.Database.MigrateAsync();
                    pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TDataProtectionDbContext>())
                {
                    await context.Database.MigrateAsync();
                    pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
                }
            }

            return pendingMigrationCount == 0;
        }

        public static async Task<bool> EnsureSeedDataAsync<TDbContext>(IServiceProvider serviceProvider)
       where TDbContext : DbContext, IDbContext
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var dataConfiguration = scope.ServiceProvider.GetRequiredService<TestData>();

                await EnsureSeedData(context, dataConfiguration);
                return true;
            }
        }

        private static async Task EnsureSeedData<TDbContext>(TDbContext context, TestData seedData)
            where TDbContext : DbContext, IDbContext
        {
            foreach (var data in seedData.TestTable)
            {
                context.TestTable.Add(new TestTable
                {
                    Name = data.Name
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
