namespace Main.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using AutoMapper;

    using Main.Contracts;
    using Main.Contracts.Repository;
    using Main.Entities;
    using Main.Entities.Common;
    using Main.Entities.Models;
    using Main.Entities.Models.Relations;
    using Main.Entities.ViewModels;
    using Main.Entities.ViewModels.Relations;
    using Main.Repository;
    using Main.Repository.Repositories;
    using Main.Services;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    using Polly;
    using Polly.CircuitBreaker;

    using Serilog;

    using X.PagedList;

    /// <summary>
    /// 
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Import certificates in the solution trust collection
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCertificate(this IServiceCollection services)
        {
            string pathToCAFile = "./CERTIFICATE_NAME.p7b";

            // ADD CA certificate to local trust store
            X509Store localTrustStore = new X509Store(StoreName.Root);
            X509Certificate2Collection certificateCollection = new X509Certificate2Collection();

            try
            {
                certificateCollection.Import(pathToCAFile);
                localTrustStore.Open(OpenFlags.ReadWrite);
                localTrustStore.AddRange(certificateCollection);
            }
            catch (Exception ex)
            {
                Console.Write("Root certificate import failed: " + ex.Message);
            }
            finally
            {
                localTrustStore.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SeedDotnetContext>(options => options.UseSqlite(connectionString));
            services.AddTransient<SeedDotnetContextSeedData>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(o =>
            {
                //o.AddPolicy("MyPolicy",
                //    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
                o.AddDefaultPolicy(builder =>
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
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
        public static void ConfigureMapper(this IServiceCollection services)
        {
            MapperConfiguration automapConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<AddressViewModel, Address>().ReverseMap();
                    cfg.CreateMap<PatientViewModel, Patient>()
                        .ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue)).ReverseMap().ForMember(
                            p => p.Dob,
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
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ISyncPolicy>(provider =>
                Policy.Handle<Exception>().CircuitBreaker(2, TimeSpan.FromMinutes(1), OnBreak, OnReset, OnHalfOpen));
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
                    c.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
                        });
                    c.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                        {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                                },
                                new string[] { }
                            }
                        });
                    c.SwaggerDoc("v1",
                        new OpenApiInfo
                        {
                            Version = "v1", Title = "Seed DotNet",
                            Description = "This is a seed project for a .Net WebApi"
                        });
                    // Set the comments path for the Swagger JSON and UI.
                    string xmlFile = $"seed_dotnet.xml";
                    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
        }

        private static void OnBreak(Exception exception, CircuitState circuitState, TimeSpan timeSpan, Context context)
        {
            ILogger logger = Log.Logger;
            logger.Error(
                $"Circuit break with state {circuitState} using {context.PolicyKey} at {context.OperationKey}, due to: {exception} in {timeSpan.TotalSeconds}.");
        }

        private static void OnHalfOpen()
        {
            ILogger logger = Log.Logger;
            logger.Error("Circuit half open");
        }

        private static void OnReset(Context context)
        {
            ILogger logger = Log.Logger;
            logger.Error($"Circuit reset for {context.PolicyKey} at {context.OperationKey}");
        }
    }
}