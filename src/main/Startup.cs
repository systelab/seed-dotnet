namespace Main
{
    using System.IO;
    using System.Text;

    using AutoMapper;

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

    // This is 
    public class Startup
    {
        private readonly IConfigurationRoot config;

        private readonly IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

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
            // Map the view model objet with the internal model
            Mapper.Initialize(config => { config.CreateMap<PatientViewModel, Patient>().ReverseMap(); });

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
                            c.OperationFilter<AddRequiredHeaderParameter>();

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
                        config.Password.RequireNonAlphanumeric = true;
                        config.Password.RequiredLength = 8;
                        config.Password.RequireDigit = true;
                        config.User.RequireUniqueEmail = true;
                    }).AddEntityFrameworkStores<SeedDotnetContext>();

            // Configure the authentication system
            services.AddAuthentication().AddCookie().AddJwtBearer(
                cfg =>
                    {
                        cfg.TokenValidationParameters =
                            new TokenValidationParameters()
                                {
                                    ValidateIssuer = false,
                                    ValidAudience = this.config["Tokens:Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(this.config["Tokens:Key"]))
                                };
                    });

            services.AddScoped<ISeedDotnetRepository, SeedDotnetRepository>();
            services.AddLogging();

            services.AddMvc(
                config =>
                    {
                        // You can configure that in production is needed Https but for other enviroments not needed
                        if (this.env.IsProduction())
                        {
                            config.Filters.Add(new RequireHttpsAttribute());
                        }
                    }).AddJsonOptions(
                config =>
                    {
                        config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
   
        }
    }
}