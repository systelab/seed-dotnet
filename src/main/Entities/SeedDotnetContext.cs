namespace main.Entities
{
    using System.Data.Common;

    using main.Entities.Models;
    using main.Entities.Models.Relations;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// 
    /// </summary>
    public class SeedDotnetContext : IdentityDbContext<UserManage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public SeedDotnetContext(DbContextOptions options)
            : base(options)
        {
        }

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
        public DbSet<Patient> Patients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void CloseConnection()
        {
            if (this.Database?.GetDbConnection() != null)
            {
                this.Database?.CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientAllergy>().HasKey(t => new { t.IdAllergy, t.IdPatient });
        }
    }
}