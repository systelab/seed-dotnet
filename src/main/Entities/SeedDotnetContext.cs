namespace Main.Entities
{

    using Main.Entities.Models;
    using Main.Entities.Models.Relations;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class SeedDotnetContext : IdentityDbContext<UserManage>
    {
        public SeedDotnetContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Allergy> Allergies { get; set; }

        public DbSet<PatientAllergy> PatientAllergies { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public void CloseConnection()
        {
            if (this.Database?.GetDbConnection() != null)
            {
                this.Database?.CloseConnection();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientAllergy>().HasKey(t => new { t.IdAllergy, t.IdPatient });
        }
    }
}