namespace main.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using Contracts;
    using Contracts.Repository;
    using Entities;
    using Entities.Common;
    using Entities.Models;
    using Entities.Models.Relations;
    using Entities.ViewModels;
    using Entities.ViewModels.Relations;
    using main.Mail;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using PagedList.Core;
    using Polly;
    using Polly.CircuitBreaker;

    using Repository;
    using Repository.Repositories;
    using Services;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// 
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(
                o => o.AddPolicy(
                    "MyPolicy",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    c.OperationFilter<SwaggerConsumesOperationFilter>();
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", new string[] { }}
                    });
                    c.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Version = "v1",
                            Title = "Seed DotNet",
                            Description = "This is a seed project for a .Net WebApi",
                            TermsOfService = "None"
                        });
                    // Set the comments path for the Swagger JSON and UI.
                    string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                    string xmlPath = Path.Combine(basePath, "seed_dotnet.xml");
                    c.IncludeXmlComments(xmlPath);
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
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
            services.AddScoped <IMailService, MailService>();
            services.AddScoped<ISyncPolicy>(provider =>
                Policy.Handle<Exception>().CircuitBreaker(2, TimeSpan.FromMinutes(1), onBreak: OnBreak, onReset: OnReset, onHalfOpen: OnHalfOpen));
        }

        private static void OnHalfOpen()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error($"Circuit half open");
        }

        private static void OnReset(Context context)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error($"Circuit reset for {context.PolicyKey} at {context.OperationKey}");
        }

        private static void OnBreak(Exception exception, CircuitState circuitState, TimeSpan timeSpan, Context context)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error($"Circuit break with state {circuitState} using {context.PolicyKey} at {context.OperationKey}, due to: {exception} in {timeSpan.TotalSeconds}.");
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureMapper(this IServiceCollection services)
        {
            MapperConfiguration automapConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddressViewModel, Address>().ReverseMap();
                cfg.CreateMap<PatientViewModel, Patient>()
                    .ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue))
                    .ReverseMap()
                    .ForMember(p => p.Dob,
                        o => o.MapFrom(q => q.Dob == DateTime.MinValue ? null : new DateTime?(q.Dob)));
                cfg.CreateMap<UserViewModel, UserManage>().ReverseMap();
                cfg.CreateMap<AllergyViewModel, Allergy>().ReverseMap();
                cfg.CreateMap<PatientAllergyViewModel, PatientAllergy>().ReverseMap();
                cfg.CreateMap<EmailViewModel, Email>().ReverseMap();

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

            IMapper mapper = automapConfiguration.CreateMapper();

            services.AddSingleton(mapper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureContext(this IServiceCollection services)
        {
            services.AddDbContext<SeedDotnetContext>();
            services.AddTransient<SeedDotnetContextSeedData>();
        }
    }
}