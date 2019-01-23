using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTest")]
[assembly: InternalsVisibleTo("test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace main
{
    using System.IO;
    using main;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;

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
             BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSetting("https_port", "13080")               
                .Build();
    }
}