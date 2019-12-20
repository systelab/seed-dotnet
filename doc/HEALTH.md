# Healthcheck

ASP.NET contains extenstions for healtcheck endpoints. Check [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2) for the full reference.

Basically, it consists in three elements:

- Adding the service endpoint with an url
- Adding the health check providers
- Optionally publishing the results to another system or implementing a minimal UI to consume the data

For instance, the healthcheck can be consumed by Docker, as it has been done in this seed. 

In this case the seed reports the health status at the endpoint `\health`

In case of a healthy state, the HTTP request will return 200
In case of an unhealthy state, the HTTP request will return 503

You can check this value with a browser.

## Custom health checks

You can (should) implement your own healthchecks based on your application logic. The seed shows an example of that with the class `ExampleHealthCheck`

## Ready-to-use health checks

There is a bunch on already implemented health checks [here](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)

The seed uses the Sqlite, the System (Disk usage) and Network (basic ping connectivity) health checks


## Publishing of status

The health checks can be published to specialized servers like [Prometheus](https://prometheus.io/). This is left as part of a future evolution of the seed.

[This page](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#healthcheck-push-results) shows some of the publishers already implemented 
