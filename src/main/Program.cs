namespace Main
{
    using System.IO;

    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder().UseKestrel().UseContentRoot(Directory.GetCurrentDirectory()).UseUrls("http://127.0.0.1:13080")
                .UseIISIntegration().UseStartup<Startup>().Build();

            host.Run();
        }
    }
}