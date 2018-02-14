namespace Main.Models
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    public class SeedDotnetContextSeedData
    {
        private SeedDotnetContext context;

        private readonly UserManager<UserManage> userManager;

        public SeedDotnetContextSeedData(SeedDotnetContext context, UserManager<UserManage> userM)
        {
            this.context = context;
            this.userManager = userM;
        }

        public async Task EnsureSeedData()
        {
            if (await this.userManager.FindByNameAsync("admin") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "admin",
                                   Name = "Administrator",
                                   LastName = "Seed_Dotnet",
                                   Email = "admin@werfen.com"
                               };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this.userManager.FindByNameAsync("quentinada") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "quentinada",
                                   Name = "quentinada",
                                   LastName = "quentinada",
                                   Email = "quentinada@werfen.com"
                               };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this.userManager.FindByNameAsync("test") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "test",
                                   Name = "test",
                                   LastName = "test_Seed_Dotnet",
                                   Email = "testadmin@werfen.com"
                               };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }
        }
    }
}