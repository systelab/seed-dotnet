namespace main.Contracts
{
    using Entities;
    using Entities.Models;

    internal interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}