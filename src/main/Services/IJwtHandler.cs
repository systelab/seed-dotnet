using Main.Models;


namespace Main.Services
{
    internal interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}
