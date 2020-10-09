namespace Main.Migrations
{
    using System;
    using System.Reflection;

    using FluentMigrator.Runner;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// </summary>
    public static class DatabaseMigrationRunner
    {
        /// <summary>
        ///     Static method that runs the migration
        /// </summary>
        /// <param name="connectionString">Example: Data Source=database.db</param>
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
            Assembly[] assemblies = { typeof(InitialDatabaseStructure).Assembly, typeof(InsertExampleData).Assembly };

            return new ServiceCollection()
                .AddLogging(logging => logging.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    configureRunner => configureRunner.AddSQLite()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(assemblies).For.Migrations())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}