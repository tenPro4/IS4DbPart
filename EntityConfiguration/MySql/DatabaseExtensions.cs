using EntityConfiguration.Configuration;
using EntityScheme;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConfiguration.MySql
{
    public static class DatabaseExtensions
    {
        public static void RegisterMySqlDbContexts<TDbContext,TDataProtectionDbContext>(this IServiceCollection services,
            ConnectionStringsConfiguration connectionStrings,
            DatabaseMigrationsConfiguration databaseMigrations)
            where TDbContext : DbContext, IDbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).Assembly.GetName().Name;

            services.AddDbContext<TDbContext>(options => options.UseMySql(connectionStrings.DbConnection, ServerVersion.AutoDetect(connectionStrings.DbConnection),
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DbConnectionMigrationsAssembly ?? migrationsAssembly)));

            if (!string.IsNullOrEmpty(connectionStrings.DataProtectionDbConnection))
                services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(connectionStrings.DataProtectionDbConnection, ServerVersion.AutoDetect(connectionStrings.DataProtectionDbConnection),
                    optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly)));
        }
    }
}
