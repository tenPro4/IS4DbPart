using DatabasePart.Configuration.Database;
using DatabasePart.Helpers;
using DatabasePart.Helpers.DependencyInjection;
using EntityShare.DbContexts;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace DatabasePart
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            HostingEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMigration<MainDbContext, DataProtectionDbContext>(ConfigureUIOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapIdentityServer4AdminUIHealthChecks();
            });
        }

        public virtual void ConfigureUIOptions(IdentityServer4AdminUIOptions options)
        {
            // Applies configuration from appsettings.
            options.BindConfiguration(Configuration);
            //if (HostingEnvironment.IsDevelopment())
            //{
            //    options.Security.UseDeveloperExceptionPage = true;
            //}
            //else
            //{
            //    options.Security.UseHsts = true;
            //}

            // Set migration assembly for application of db migrations
            var migrationsAssembly = MigrationAssemblyConfiguration.GetMigrationAssemblyByProvider(options.DatabaseProvider);
            options.DatabaseMigrations.SetMigrationsAssemblies(migrationsAssembly);

            // Use production DbContexts and auth services.
            //options.Testing.IsStaging = false;
        }

    }
}
