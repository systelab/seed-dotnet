namespace Main.Contracts
{
    using System.Threading.Tasks;

    using Main.Entities;

    /// <summary>
    ///     Service to manage logins
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        ///     Refresh the token
        /// </summary>
        /// <param name="token">token to refresh</param>
        /// <returns>the token refreshed</returns>
        Task<JsonWebToken> RefreshAccessToken(string token);

        /// <summary>
        ///     Login method
        /// </summary>
        /// <param name="username">the user name</param>
        /// <param name="password">the user password</param>
        /// <returns>The token</returns>
        Task<JsonWebToken> SignIn(string username, string password);
    }
}