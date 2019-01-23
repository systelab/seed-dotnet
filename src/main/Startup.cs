namespace main
{
    using System.Text;



    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using Newtonsoft.Json.Serialization;
    using main.Extensions;

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
                services.ConfigureSwagger();
            }

            // Allow use the API from other origins 
            services.ConfigureCors();

            // Set the context to the database
            services.ConfigureContext();

            // Set Identity
            services.ConfigureIdentity();

            //Configure the Scopes
            services.ConfigureScope();


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

           //Configure Logging
            services.AddLogging();

            //Configure Mappers
            services.ConfigureMapper();

            services.AddMvc().AddJsonOptions(
                config =>
                    {
                        config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
   
        }
    }
}