namespace Main.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class SeedDotnetContext : IdentityDbContext<UserManage>
    {
        private readonly IConfigurationRoot config;

        public SeedDotnetContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            this.config = config;
        }

        public DbSet<Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(this.config["ConnectionStrings:seed_dotnetContextConnection"]);
        }
    }
}