using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;

using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.AspNetCore.Identity;
using main.Entities;
using main.Contracts;
using main.Services;
using main.Entities.Models;
using main.Repository;
using Polly;
using System;
using main.Entities.ViewModels;
using PagedList.Core;
using main.Entities.Common;
using System.Linq;
using System.Threading.Tasks;
using main.Contracts.Repository;
using main.Repository.Repositories;
using main.Entities.Models.Relations;
using main.Entities.ViewModels.Relations;

namespace main.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(
                o => o.AddPolicy(
                    "MyPolicy",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials(); }));
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
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
        public static void ConfigureIdentity(this IServiceCollection services)
        {
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
        }
        public static void ConfigureScope(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IAllergyRepository, AllergyRepository>();
            services.AddScoped<IMedicalRecordNumberService, MedicalRecordNumberService>();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IPasswordHasher<UserManage>, PasswordHasher<UserManage>>();
            services.AddScoped<ISeedDotnetRepository, SeedDotnetRepository>();
            services.AddScoped<ISyncPolicy>(provider =>
                Policy.Handle<Exception>().CircuitBreaker(2, TimeSpan.FromMinutes(1)));

        }

        public static void ConfigureMapper(this IServiceCollection services)
        {
            var automapConfiguration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddressViewModel, Address>().ReverseMap();
                cfg.CreateMap<PatientViewModel, Patient>()
                    .ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue))
                    .ReverseMap()
                    .ForMember(p => p.Dob, o => o.MapFrom(q => (q.Dob == DateTime.MinValue) ? null : new DateTime?(q.Dob)));
                cfg.CreateMap<UserViewModel, UserManage>().ReverseMap();
                cfg.CreateMap<AllergyViewModel, Allergy>().ReverseMap();
                cfg.CreateMap<PatientAllergyViewModel, PatientAllergy>().ReverseMap();
                
                #region Pagination configurations

                cfg.CreateMap<PagedList<Patient>, ExtendedPagedList<PatientViewModel>>()
                    .ForMember(p => p.TotalPages, o => o.MapFrom(q => q.PageCount))
                    .ForMember(p => p.Content, o => o.MapFrom(q => q.AsEnumerable()))
                    .ForMember(p => p.First, o => o.MapFrom(q => q.IsFirstPage))
                    .ForMember(p => p.Last, o => o.MapFrom(q => q.IsLastPage))
                    .ForMember(p => p.Size, o => o.MapFrom(q => q.PageSize))
                    .ForMember(p => p.NumberOfElements, o => o.MapFrom(q => q.Count))
                    .ForMember(p => p.Number, o => o.MapFrom(q => q.PageNumber - 1))
                    .ForMember(p => p.TotalElements, o => o.MapFrom(q => q.TotalItemCount));

                cfg.CreateMap<PagedList<Allergy>, ExtendedPagedList<AllergyViewModel>>()
                    .ForMember(p => p.TotalPages, o => o.MapFrom(q => q.PageCount))
                    .ForMember(p => p.Content, o => o.MapFrom(q => q.AsEnumerable()))
                    .ForMember(p => p.First, o => o.MapFrom(q => q.IsFirstPage))
                    .ForMember(p => p.Last, o => o.MapFrom(q => q.IsLastPage))
                    .ForMember(p => p.Size, o => o.MapFrom(q => q.PageSize))
                    .ForMember(p => p.NumberOfElements, o => o.MapFrom(q => q.Count))
                    .ForMember(p => p.Number, o => o.MapFrom(q => q.PageNumber - 1))
                    .ForMember(p => p.TotalElements, o => o.MapFrom(q => q.TotalItemCount));


                #endregion
            });

            var mapper = automapConfiguration.CreateMapper();

            services.AddSingleton(mapper);
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
          
        }

        public static void ConfigureContext(this IServiceCollection services)
        {
            services.AddDbContext<SeedDotnetContext>();
            services.AddTransient<SeedDotnetContextSeedData>();
        }
    }
}

