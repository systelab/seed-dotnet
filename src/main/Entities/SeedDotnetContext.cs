namespace main.Entities
{
    using System;
    using System.Data.Common;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Models.Relations;

    /// <summary>
    /// 
    /// </summary>
    public class SeedDotnetContext : IdentityDbContext<UserManage>
    {
        private readonly IConfigurationRoot config;
        private SqliteConnection connection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="options"></param>
        public SeedDotnetContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            this.config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Patient> Patients { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Allergy> Allergies { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<PatientAllergy> PatientAllergies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(this.GetConnection());
        }

        private DbConnection GetConnection()
        {
            this.connection = new SqliteConnection(this.config["ConnectionStrings:seed_dotnetContextConnection"]);
            this.connection.Open();

            return this.connection;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DbConnection CloseConnection()
        {
            if (this.connection != null)
            {
                this.connection.Close();
                return this.connection;
            }

            return null;
        }

        private string GetPassword()
        {
            // modify this code to retrieve the password from a secure key repository. Obviously, avoid using the configuration file!
            return this.config["ConnectionStrings:password"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientAllergy>()
                .HasKey(t => new { t.IdAllergy, t.IdPatient });
        }
    }
}