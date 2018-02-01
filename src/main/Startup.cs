using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using System.Web.Http.Cors;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using seed_dotnet.Models;
using seed_dotnet.Services;
using seed_dotnet.ViewModels;

namespace seed_dotnet
{
    //This is 
    public class Startup
    {

        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
              .SetBasePath(_env.ContentRootPath)
              .AddJsonFile("appsettings.json")
              .AddEnvironmentVariables();

            _config = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            if (_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
               //Here you can set the services implemented only for DEV and TEST
            }
            else
            {
                //Here you can set the services implemented only for Prodcution
            }

            //Allow use the API from other origins 
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }
           ));

            //Set the context to the database
            services.AddDbContext<seed_dotnetContext>();

            //Set
            services.AddIdentity<UserManage, IdentityRole>(config =>
            {
               
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireDigit = true;
                config.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<seed_dotnetContext>();

            //Configure the authentication system
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidAudience = _config["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                    };
                });

            services.AddScoped<ISeed_dotnetRepository, Seed_dotnetRepository>();
            services.AddTransient<seed_dotnetContextSeeData>();
            services.AddLogging();

            services.AddMvc(config =>
            {
                //You can configure that in production is needed Https but for other enviroments not needed
                if (_env.IsProduction())
                {
                    config.Filters.Add(new RequireHttpsAttribute());
                }
            })

              .AddJsonOptions(config =>
              {

                  config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

              });

            //Add Swagger reference to the project
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AddRequiredHeaderParameter>();

                c.SwaggerDoc("v1", new Info
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
       IHostingEnvironment env,
       ILoggerFactory factory,seed_dotnetContextSeeData seeder )
        {
            //Map the view model objet with the internal model
            Mapper.Initialize(config =>
            {
                config.CreateMap<PatientViewModel, Patient>().ReverseMap();
            });

            //Configure how to display the errors and the level of severity
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
            {
                factory.AddDebug(LogLevel.Error);
            }

            app.UseCors("MyPolicy");

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "index" }
                    );
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seed .Net");
            });
             seeder.EnsureSeedData().Wait();
        }
    }
}
