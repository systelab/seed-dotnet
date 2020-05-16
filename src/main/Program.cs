using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTest")]
[assembly: InternalsVisibleTo("test")]
[assembly: InternalsVisibleTo("TestNUnit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace main
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    using NLog.Web;

    /// <summary>
    ///     Main entry program
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Main entry
        /// </summary>
        /// <param name="args">not used</param>
        public static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "seed-dotnet")))
            {
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "seed-dotnet"));
            }

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSetting("https_port", "13080")
                .ConfigureLogging(
                    logging =>
                        {
                            logging.ClearProviders();
                            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        })
                .UseNLog()
                .Build();

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
            //    .ConfigureLogging(
            //        logging =>
            //        {
            //            logging.ClearProviders();
            //            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            //        })
            //    .UseNLog()
            //    .Build();
        }
    }
}