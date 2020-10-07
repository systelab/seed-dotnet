namespace main.Extensions
{
    using System.Threading.Tasks;

    using main.Entities.Models;

    using Microsoft.AspNetCore.Identity;

    internal class SeedDotnetContextSeedData
    {
        private readonly UserManager<UserManage> userManager;

        public SeedDotnetContextSeedData(UserManager<UserManage> _userM)
        {
            this.userManager = _userM;
        }

        public async Task EnsureSeedData()
        {
            if (await this.userManager.FindByNameAsync("Systelab") == null)
            {
                UserManage user = new UserManage { UserName = "Systelab", Name = "Systelab", LastName = "Seed_Dotnet", Email = "Systelab@werfen.com" };
                await this.userManager.CreateAsync(user, "Systelab");
            }

            if (await this.userManager.FindByNameAsync("admin") == null)
            {
                UserManage user = new UserManage { UserName = "admin", Name = "Administrator", LastName = "Seed_Dotnet", Email = "admin@werfen.com" };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this.userManager.FindByNameAsync("quentinada") == null)
            {
                UserManage user = new UserManage { UserName = "quentinada", Name = "quentinada", LastName = "quentinada", Email = "quentinada@werfen.com" };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this.userManager.FindByNameAsync("test") == null)
            {
                UserManage user = new UserManage { UserName = "test", Name = "test", LastName = "test_Seed_Dotnet", Email = "testadmin@werfen.com" };
                await this.userManager.CreateAsync(user, "P@ssw0rd!");
            }
        }
    }
}