namespace main
{
    using System;
    using System.IO.Compression;
    using System.Text;
    using System.Text.Json.Serialization;
    using global::Hangfire;
    using global::Hangfire.SQLite;
    using main.Entities;
    using main.Extensions;
    using main.Hangfire;
    using main.Healthchecks;
    using main.Migrations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Serilog;

    // This is 
    internal class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;

            //Migrations
            DatabaseMigrationRunner.Start(this.Configuration.GetConnectionString("ContextConnection"));
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            // Configure how to display the errors and the level of severity
            if (this.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            if (!this.Environment.IsProduction())
            {
                app.UseSwagger();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(x => 
                x.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.Use(
                (context, next) =>
                {
                    context.Response.Headers["Access-Control-Expose-Headers"] =
                        "origin, content-type, accept, authorization, ETag, if-none-match";
                    context.Response.Headers["Access-Control-Max-Age"] = "1209600";
                    context.Response.Headers["Access-Control-Allow-Methods"] =
                        "GET, POST, PUT, DELETE, OPTIONS, HEAD, PATCH";
                    context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                    context.Response.Headers["Access-Control-Allow-Headers"] =
                        "origin, content-type, accept, authorization, Etag, if-none-match";
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                    context.Response.Headers["X-Frame-Options"] = "deny";
                    context.Response.Headers["Strict-Transport-Security"] = "max-age=300; includeSubDomains";
                    return next.Invoke();
                });

            app.UseResponseCompression();
            app.UseHealthChecks("/health");

            // Enable middleware to serve generated Swagger as a JSON endpoint.

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seed .Net"); });

            app.UseHangfireDashboard("/hangfire",
                new DashboardOptions {Authorization = new[] {new HangFireAuthenticationFilter()}});

            app.UseHangfireServer(new BackgroundJobServerOptions {WorkerCount = 1});

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute {Attempts = 0});
            HangfireJobScheduler.ScheduleRecurringJobs();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = this.Configuration.GetConnectionString("ContextConnection");
            
            services.ConfigureContext(connectionString);

            IConfigurationSection appSettingsSection = this.Configuration.GetSection("AppSettings");

            AppSettingsModel appSettingsModel = appSettingsSection.Get<AppSettingsModel>();

            services.AddSingleton(appSettingsModel);
            

            if (this.Environment.IsDevelopment())
            {
                // Here you can set the services implemented only for DEV 
            }

            if (!this.Environment.IsProduction())
                // Add Swagger reference to the project. Swagger is not needed when testing
            {
                services.ConfigureSwagger();
            }

            services.AddCors();
            services.AddControllers();

            // Allow use the API from other origins 
            //services.ConfigureCors();


            // Set the context to the database
            
            services.AddHealthChecks();

            // Set Identity
            services.ConfigureIdentity();

            //Configure the Scopes
            
            services.ConfigureScope();
            

            // Add certificates
            services.ConfigureCertificate();

            // Configure the authentication system
            services.AddAuthentication(
                x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                bearerOptions =>
                {
                    bearerOptions.RequireHttpsMetadata = false;
                    bearerOptions.SaveToken = true;
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettingsModel.Secret)),
                        ValidateAudience = false,
                        ValidIssuer = appSettingsModel.Issuer,
                        ValidateIssuer = false
                    };
                });

            //Configure Mappers
            services.ConfigureMapper();

            // Configure Compression level
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            // Add Response compression services
            services.AddResponseCompression(
                options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    options.EnableForHttps = true;
                });
            // add some healthchecks
            services.AddHealthChecks().AddCheck<ExampleHealthCheck>("exampleHealthCheck")
                .AddSqlite(this.Configuration.GetConnectionString("ContextConnection"))
                .AddDiskStorageHealthCheck(options => options.AddDrive(@"C:\", 1000))
                .AddPingHealthCheck(options => options.AddHost("www.google.com", 1000));

            services.AddMvcCore().AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                }).AddApiExplorer();
            //Versioning of API
            services.AddApiVersioning(
                o =>
                {
                    o.ReportApiVersions = true;
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                });

            //Add HangFire
            services.AddHangfire(
                config =>
                {
                    SQLiteStorageOptions options = new SQLiteStorageOptions
                        {PrepareSchemaIfNecessary = true, QueuePollInterval = TimeSpan.FromMinutes(5)};
                    config.UseSQLiteStorage(this.Configuration.GetConnectionString("ContextConnection"), options);
                });
        }
    }
}