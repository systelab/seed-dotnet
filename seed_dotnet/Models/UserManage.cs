using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace seed_dotnet.Models
{
    /// <summary>
    /// UserManage is a extension of the information provided by the IdentityUser class
    /// </summary>
    public class UserManage : IdentityUser
    {
        public string LastName { get; set; }
        public string Name { get; set; }
    }
}
