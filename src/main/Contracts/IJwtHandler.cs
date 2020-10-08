namespace Main.Contracts
{
    using Main.Entities;
    using Main.Entities.Models;

    internal interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}