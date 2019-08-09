namespace main.Healthchecks
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class ExampleHealthCheck : IHealthCheck
    {
        public ExampleHealthCheck()
        {
            // Use DI to inject services here
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool healthCheckResultHealthy = true;

            // Do your logic here 

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The check indicates a healthy result."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
        }
    }
}