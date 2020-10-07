namespace main.Healthchecks
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class ExampleHealthCheck : IHealthCheck
    {
        private readonly IConfigurationRoot configuration;

        public ExampleHealthCheck(IConfigurationRoot configuration)
        {
            // Use DI to inject services here
            this.configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            // Do your logic here 
            bool healthCheckResultHealthy = this.configuration != null;

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The check indicates a healthy result."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
        }
    }
}