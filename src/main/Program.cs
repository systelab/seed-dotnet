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
            //var host = BuildWebHost().Seed();

            //host.Run();
             BuildWebHost(args).Run();
        }

        //private static IWebHost BuildWebHost()
        //{
        //    return new WebHostBuilder()
        //        .UseKestrel()
        //        .UseContentRoot(Directory.GetCurrentDirectory()).UseUrls("http://0.0.0.0:13080")
        //        .UseIISIntegration()
        //        .UseStartup<Startup>()
        //        .Build();
        //}
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}