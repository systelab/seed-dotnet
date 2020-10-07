namespace main
{
    using System;
    using System.IO.Compression;
    using System.Text;
    using Extensions;
    using global::Hangfire;
    using global::Hangfire.SQLite;
    using main.Hangfire;
    using HealthChecks.Network;
    using main.Healthchecks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Migrations;
    using Newtonsoft.Json.Serialization;
    using SQLitePCL;

    // This is 
    internal class Startup
    {
        private readonly IConfigurationRoot config;

        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                this.env = env;
                Batteries_V2.Init();

                IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(this.env.ContentRootPath)
                    .AddJsonFile("appsettings.json").AddEnvironmentVariables();

                this.config = builder.Build();

                //Migrations
                DatabaseMigrationRunner.Start(this.config["ConnectionStrings:seed_dotnetContextConnection"]);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory factory)
        {
            // Configure how to display the errors and the level of severity
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
            {
                app.UseHsts();
                factory.AddDebug(LogLevel.Error);
            }

            if (!env.IsEnvironment("Testing"))
            {
                factory.AddDebug(LogLevel.Information);
                app.UseSwagger();
            }
            else
            {
                factory.AddConsole();
            }


            app.UseCors("MyPolicy");

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.Use((context, next) =>
            {
                context.Response.Headers["Access-Control-Expose-Headers"] = "origin, content-type, accept, authorization, ETag, if-none-match";
                context.Response.Headers["Access-Control-Max-Age"] = "1209600";
                context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, HEAD, PATCH";
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                context.Response.Headers["Access-Control-Allow-Headers"] = "origin, content-type, accept, authorization, Etag, if-none-match";
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["X-Frame-Options"] = "deny";
                context.Response.Headers["Strict-Transport-Security"] = "max-age=300; includeSubDomains";
                return next.Invoke();
            });

            app.UseResponseCompression();
            app.UseHealthChecks("/health");

            app.UseMvc(
                config =>
                {
                    config.MapRoute(
                        "Default",
                        "{controller}/{action}/{id?}",
                        new {controller = "Home", action = "index"});
                });

            // Enable middleware to serve generated Swagger as a JSON endpoint.


            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seed .Net"); });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthenticationFilter() }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1,
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            HangfireJobScheduler.ScheduleRecurringJobs();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.config);
            if (this.env.IsEnvironment("Development"))
            {
                // Here you can set the services implemented only for DEV 
            }

            if (!this.env.IsEnvironment("Testing"))
                // Add Swagger reference to the project. Swagger is not needed when testing
            {
                services.ConfigureSwagger();
            }

            // Allow use the API from other origins 
            services.ConfigureCors();

            // Set the context to the database
            services.ConfigureContext();

            services.AddHealthChecks();

            // Set Identity
            services.ConfigureIdentity();

            //Configure the Scopes
            services.ConfigureScope();

            // Add certificates
            services.ConfigureCertificate();

            // Configure the authentication system
            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["jwt:secretKey"])),
                        ValidIssuer = this.config["jwt:issuer"],
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });


            //Configure Mappers
            services.ConfigureMapper();

            // Configure Compression level
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            // Add Response compression services
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });
            // add some healthchecks
            services.AddHealthChecks().AddCheck<ExampleHealthCheck>("exampleHealthCheck")
                .AddSqlite(this.config["ConnectionStrings:seed_dotnetContextConnection"])
                .AddDiskStorageHealthCheck(options => options.AddDrive(@"C:\", minimumFreeMegabytes: 1000))
                .AddPingHealthCheck(options => options.AddHost("www.google.com", 1000));

            services.AddMvc().AddJsonOptions(
                config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            //Versioning of API
            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            //Add HangFire
            services.AddHangfire(config =>
            {
                var options = new SQLiteStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    QueuePollInterval = TimeSpan.FromMinutes(5)
                };
                config.UseSQLiteStorage(this.config["ConnectionStrings:seed_dotnetContextConnection"], options);


            });
        }
    }
}