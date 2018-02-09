namespace seed_dotnet.Models
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    public class seed_dotnetContextSeeData
    {
        private seed_dotnetContext _context;

        private UserManager<UserManage> _userManager;

        public seed_dotnetContextSeeData(seed_dotnetContext context, UserManager<UserManage> userM)
        {
            this._context = context;
            this._userManager = userM;
        }

        public async Task EnsureSeedData()
        {
            if (await this._userManager.FindByNameAsync("admin") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "admin",
                                   Name = "Administrator",
                                   LastName = "Seed_Dotnet",
                                   Email = "admin@werfen.com"
                               };
                await this._userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this._userManager.FindByNameAsync("quentinada") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "quentinada",
                                   Name = "quentinada",
                                   LastName = "quentinada",
                                   Email = "quentinada@werfen.com"
                               };
                await this._userManager.CreateAsync(user, "P@ssw0rd!");
            }

            if (await this._userManager.FindByNameAsync("test") == null)
            {
                var user = new UserManage()
                               {
                                   UserName = "test",
                                   Name = "test",
                                   LastName = "test_Seed_Dotnet",
                                   Email = "testadmin@werfen.com"
                               };
                await this._userManager.CreateAsync(user, "P@ssw0rd!");
            }
        }
    }
}