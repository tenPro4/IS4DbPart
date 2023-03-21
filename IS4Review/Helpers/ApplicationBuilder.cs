using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilder
    {
        public static IEndpointConventionBuilder MapIdentityServer4AdminUIHealthChecks(this IEndpointRouteBuilder endpoint, string pattern = "/health", Action<HealthCheckOptions> configureAction = null)
        {
            var options = new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            };

            configureAction?.Invoke(options);

            return endpoint.MapHealthChecks(pattern, options);
        }
    }
}
