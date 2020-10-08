namespace Main
{
    using Main.Extensions;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    internal static class WebHostExtensions
    {
        public static IWebHost Seed(this IWebHost webhost)
        {
            using (IServiceScope scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                ContextData seeder = scope.ServiceProvider.GetRequiredService<ContextData>();
                seeder.EnsureData().Wait();
            }

            return webhost;
        }
    }
}