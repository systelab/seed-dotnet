using System;

namespace main.Migrations
{
    using System.Reflection;
    using FluentMigrator.Runner;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// 
    /// </summary>
    public static class DatabaseMigrationRunner
    {
        public static void Start(string connectionString)
        {
            IServiceProvider serviceProvider = CreateServices(connectionString);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            Assembly[] assemblies = {
                typeof(InitialDatabaseStructure).Assembly
            };

            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(configureRunner => configureRunner.
                    AddSQLite()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(assemblies).For.Migrations())
                .AddLogging(logging => logging.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}