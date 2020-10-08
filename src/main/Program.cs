namespace Main
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    using Serilog;
    using Serilog.Events;

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
            LoggerConfiguration conf = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Information).Enrich.FromLogContext()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File(GetLogFile(), LogEventLevel.Debug, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1073741824));

            Log.Logger = conf.CreateLogger();

            try
            {
                Log.Logger.Debug("init main");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseSerilog().UseStartup<Startup>().UseSetting("https_port", "13080").Build();

            ////Example of how to import a pfx certificate to serve the application in https using Kestrel

            //var cert = new X509Certificate2("./CERTIFICATE_NAME.pfx", "CERTIFICATE_PASSWORD");
            //return new WebHostBuilder()
            //     .UseKestrel(options =>
            //     {
            //         options.Listen(IPAddress.Any, 443, listenOptions =>
            //         {
            //             listenOptions.UseHttps(cert);

            //         });

            //     })
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseStartup<Startup>()
            //      (...)
            //    .Build();
        }

        private static string GetLogFile()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, $"{Path.GetFileName(AppDomain.CurrentDomain.FriendlyName)}.log");
        }
    }
}