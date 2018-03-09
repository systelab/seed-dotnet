namespace Main
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    public static class Program
    {
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