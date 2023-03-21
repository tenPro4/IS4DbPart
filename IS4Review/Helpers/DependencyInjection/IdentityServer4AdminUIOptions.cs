using EntityConfiguration.Configuration;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;
using DatabasePart.Configuration.Constants;

namespace DatabasePart.Helpers.DependencyInjection
{
    public class IdentityServer4AdminUIOptions
    {
        public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new ConnectionStringsConfiguration();
        public DatabaseProviderConfiguration DatabaseProvider { get; set; } = new DatabaseProviderConfiguration();
        public DatabaseMigrationsConfiguration DatabaseMigrations { get; set; } = new DatabaseMigrationsConfiguration();
        public TestData TestData { get; set; } = new TestData();
        public Func<IServiceCollection, IHealthChecksBuilder> HealthChecksBuilderFactory { get; set; }

        public void BindConfiguration(IConfiguration configuration)
        {
            configuration.GetSection(ConfigurationConsts.ConnectionStringsKey).Bind(ConnectionStrings);
            configuration.GetSection(nameof(DatabaseProviderConfiguration)).Bind(DatabaseProvider);
            configuration.GetSection(nameof(DatabaseMigrationsConfiguration)).Bind(DatabaseMigrations);
            configuration.GetSection(nameof(TestData)).Bind(TestData);
        }

    }
}
