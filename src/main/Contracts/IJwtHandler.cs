

using main.Entities;
using main.Entities.Models;

namespace main.Contracts
{
    internal interface IJwtHandler
    {
        JsonWebToken Create(UserManage user);
    }
}
