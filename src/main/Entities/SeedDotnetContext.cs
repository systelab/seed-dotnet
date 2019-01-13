namespace main.Entities
{
    using System;
    using System.Data.Common;
    using System.Security;
    using main.Entities.Models;
    using main.Entities.Models.Relations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class SeedDotnetContext : IdentityDbContext<UserManage>
    {
        private readonly IConfigurationRoot config;
        private SqliteConnection connection;

        public SeedDotnetContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            this.config = config;
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(this.GetConnection());
        }

        private DbConnection GetConnection()
        {
            this.connection =
                new SqliteConnection(this.config["ConnectionStrings:seed_dotnetContextConnection"]);
            // each connection will use the password for unencrypt the database.
            // The following code executes the PRAGMA with two SQL Queries to prevent SQL-injection in the password
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            //command.CommandText = "SELECT quote($password);";
            //command.Parameters.AddWithValue("$password", this.GetPassword());
            //string quotedPassword = (string)command.ExecuteScalar();
            //command.CommandText = "PRAGMA key = " + quotedPassword;
            //command.Parameters.Clear();
            //command.ExecuteNonQuery();


            return connection;
        }

        public DbConnection CloseConnection()
        {
            if (this.connection != null)
            {
                this.connection.Close();
                return this.connection;
            }
            else
            {
                return null;
            }
        }

        private string GetPassword()
        {
            // modify this code to retrieve the password from a secure key repository. Obviously, avoid using the configuration file!
            return this.config["ConnectionStrings:password"];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientAllergy>()
               .HasKey(t => new { t.IdAllergy, t.IdPatient });

        }
    }
}