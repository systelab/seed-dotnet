using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seed_dotnet.Models
{
    public class seed_dotnetContextSeeData
    {
        private seed_dotnetContext _context;
        private UserManager<UserManage> _userManager;


        public seed_dotnetContextSeeData(seed_dotnetContext context, UserManager<UserManage> userM)
        {
            _context = context;
            _userManager = userM;
        }


        public async Task EnsureSeedData()
        {



            if (await _userManager.FindByNameAsync("admin") == null)
            {
                var user = new UserManage()
                {
                    UserName = "admin",
                    Name="Administrator",
                    LastName="Seed_Dotnet",
                    Email = "admin@werfen.com"
                };
                await _userManager.CreateAsync(user, "P@ssw0rd!");
            }


            if (await _userManager.FindByNameAsync("test") == null)
            {
                var user = new UserManage()
                {
                    UserName = "test",
                    Name = "test",
                    LastName = "test_Seed_Dotnet",
                    Email = "testadmin@werfen.com"
                };
                await _userManager.CreateAsync(user, "P@ssw0rd!");
            }


        }
    }
}
