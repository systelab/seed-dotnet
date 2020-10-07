namespace main.Contracts
{
    using main.Entities;
    using main.Entities.Models;

    internal interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}