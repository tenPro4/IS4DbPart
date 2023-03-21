using EntityConfiguration.Configuration;
using EntityScheme;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityConfiguration.PostgreSQL
{
    public static class DatabaseExtensions
    {
        public static void RegisterNpgSqlDbContexts<TDbContext, TDataProtectionDbContext>(this IServiceCollection services,
           ConnectionStringsConfiguration connectionStrings,
           DatabaseMigrationsConfiguration databaseMigrations)
           where TDbContext : DbContext, IDbContext
           where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Log DB from existing connection
            services.AddDbContext<TDbContext>(options => options.UseNpgsql(connectionStrings.DbConnection,
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DbConnectionMigrationsAssembly ?? migrationsAssembly)));

            // DataProtectionKey DB from existing connection
            if (!string.IsNullOrEmpty(connectionStrings.DataProtectionDbConnection))
                services.AddDbContext<TDataProtectionDbContext>(options => options.UseNpgsql(connectionStrings.DataProtectionDbConnection, sql => sql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly)));
        }
    }
}
