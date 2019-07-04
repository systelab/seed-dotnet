namespace main.Contracts
{
    using System.Threading.Tasks;
    using Entities.Models;

    /// <summary>
    ///     Interface with all the method needed
    /// </summary>
    public interface ISeedDotnetRepository
    {
        Task<UserManage> GetUserManageWithRefreshToken(string token);

        Task UpdateRefreshToken(UserManage user);
    }
}