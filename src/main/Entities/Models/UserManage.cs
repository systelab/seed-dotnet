namespace main.Entities.Models
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// UserManage is a extension of the information provided by the IdentityUser class
    /// </summary>
    public class UserManage : IdentityUser
    {
        public string LastName { get; set; }

        public string Name { get; set; }

        public string RefreshToken { get; set; }
    }
}