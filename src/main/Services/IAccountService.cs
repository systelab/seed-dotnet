using Main.Models;
using System.Threading.Tasks;

namespace Main.Services
{
    /// <summary>
    /// Service to manage logins
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="username">the user name</param>
        /// <param name="password">the user password</param>
        /// <returns>The token</returns>
        Task<JsonWebToken> SignIn(string username, string password);

        /// <summary>
        /// Refresh the token
        /// </summary>
        /// <param name="token">token to refresh</param>
        /// <returns>the token refreshed</returns>
        Task<JsonWebToken> RefreshAccessToken(string token);
    }
}
