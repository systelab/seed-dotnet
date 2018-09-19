namespace Main
{
    using Main.Models;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    internal static class WebHostExtensions
    {
        public static IWebHost Seed(this IWebHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<SeedDotnetContextSeedData>();
                seeder.EnsureSeedData().Wait();
            }

            return webhost;
        }

    }
}