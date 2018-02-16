using Main.Models;
using System.Threading.Tasks;

namespace Main.Services
{
    public interface IAccountService
    {
        Task<JsonWebToken> SignIn(string username, string password);
        Task<JsonWebToken> RefreshAccessToken(string token);
    }
}
