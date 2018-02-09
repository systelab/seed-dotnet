namespace seed_dotnet.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class seed_dotnetContext : IdentityDbContext<UserManage>
    {
        private IConfigurationRoot _config;

        public seed_dotnetContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            this._config = config;
        }

        public DbSet<Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(this._config["ConnectionStrings:seed_dotnetContextConnection"]);
        }
    }
}