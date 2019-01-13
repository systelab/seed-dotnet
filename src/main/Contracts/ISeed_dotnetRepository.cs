namespace main.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using main.Entities.Models;
    using PagedList.Core;

    /// <summary>
    /// Interface with all the method needed
    /// </summary>
    public interface ISeedDotnetRepository
    {

        Task<UserManage> GetUserManageWithRefreshToken(string token);
        
        Task UpdateRefreshToken(UserManage user);
    }
}