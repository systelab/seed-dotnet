using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTest")]
[assembly: InternalsVisibleTo("test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Main
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Main entry program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry
        /// </summary>
        /// <param name="args">not used</param>
        public static void Main(string[] args)
        {
            var host = BuildWebHost().Seed();

            host.Run();
        }

        private static IWebHost BuildWebHost()
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory()).UseUrls("http://0.0.0.0:13080")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
        }
    }
}