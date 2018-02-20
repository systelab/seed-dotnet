using Main.Models;


namespace Main.Services
{
    public interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}
