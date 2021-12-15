namespace Main
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Main entry program
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerConfiguration conf = new LoggerConfiguration()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File(GetLogFile(), LogEventLevel.Debug, rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1073741824))
                .Enrich.FromLogContext();

            Log.Logger = conf.CreateLogger();
            SQLitePCL.Batteries.Init();
            try
            {
                Log.Logger.Debug("init main");
                WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                Startup startup = new Startup(builder.Configuration);

                startup.ConfigureServices(builder.Services, builder.Environment);
                WebApplication app = builder.Build();

                startup.Configure(app, app.Environment);

                app.Run();
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

        private static string GetLogFile()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, $"{Path.GetFileName(AppDomain.CurrentDomain.FriendlyName)}.log");
        }
    }
}