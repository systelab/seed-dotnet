using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace seed_dotnet.Models
{
    public class seed_dotnetContext : IdentityDbContext<UserManage>
    {
        private IConfigurationRoot _config;

        public seed_dotnetContext(IConfigurationRoot config, DbContextOptions options)
        : base(options)
        {
            _config = config;

        }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(_config["ConnectionStrings:seed_dotnetContextConnection"]);
        }
    }
}
