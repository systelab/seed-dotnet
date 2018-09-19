namespace Main
{
    using System;
    using System.IO;
    using System.Text;

    using Main.Models;
    using Main.Services;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.IdentityModel.Tokens;

    using Newtonsoft.Json.Serialization;

    using Swashbuckle.AspNetCore.Swagger;
    using System.Collections.Generic;
    using main.Models;

    // This is 
    internal class Startup
    {
        private readonly IConfigurationRoot config;

        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;
            SQLitePCL.Batteries_V2.Init();

            var builder = new ConfigurationBuilder().SetBasePath(this.env.ContentRootPath)
                .AddJsonFile("appsettings.json").AddEnvironmentVariables();

            this.config = builder.Build();
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

            app.UseAuthentication();

            app.UseMvc(
                config =>
                    {
                        config.MapRoute(
                            name: "Default",
                            template: "{controller}/{action}/{id?}",
                            defaults: new { controller = "Home", action = "index" });
                    });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seed .Net"); });
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
            {
                // Add Swagger reference to the project. Swagger is not needed when testing

                services.AddSwaggerGen(
                    c =>
                        {
                            c.OperationFilter<SwaggerConsumesOperationFilter>();
                            c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                            {
                                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                                Name = "Authorization",
                                In = "header",
                                Type = "apiKey"
                            });
                            c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                                {
                                    { "Bearer", new string[] { } }
                                });
                            c.SwaggerDoc(
                                "v1",
                                new Info
                                    {
                                        Version = "v1",
                                        Title = "Seed DotNet",
                                        Description = "This is a seed project for a .Net WebApi",
                                        TermsOfService = "None",
                                    });
                            // Set the comments path for the Swagger JSON and UI.
                            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                            var xmlPath = Path.Combine(basePath, "seed_dotnet.xml");
                            c.IncludeXmlComments(xmlPath);
                        });
            }
            

            // Allow use the API from other origins 
            services.AddCors(
                o => o.AddPolicy(
                    "MyPolicy",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials(); }));

            // Set the context to the database
            services.AddDbContext<SeedDotnetContext>();
            services.AddTransient<SeedDotnetContextSeedData>();
            // Set
            services.AddIdentity<UserManage, IdentityRole>(
                config =>
                    {
                        config.Password.RequireLowercase = true;
                        config.Password.RequireUppercase = true;
                        config.Password.RequireNonAlphanumeric = false;
                        config.Password.RequiredLength = 8;
                        config.Password.RequireDigit = false;
                        config.User.RequireUniqueEmail = false;
                    }).AddEntityFrameworkStores<SeedDotnetContext>();

            // Configure the authentication system
            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["jwt:secretKey"])),
                        ValidIssuer = this.config["jwt:issuer"],
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IPasswordHasher<UserManage>, PasswordHasher<UserManage>>();
            services.AddScoped<ISeedDotnetRepository, SeedDotnetRepository>();
            services.AddLogging();

            var automapConfiguration = new SeedMapperConfiguration();
                
            var mapper = automapConfiguration.CreateMapper();

            services.AddSingleton(mapper);
            services.AddMvc().AddJsonOptions(
                config =>
                    {
                        config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
   
        }
    }
}