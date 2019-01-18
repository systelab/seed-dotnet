namespace main.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using main.Contracts;
    using main.Entities;
    using main.Entities.Models;
    using Microsoft.EntityFrameworkCore;

    using PagedList.Core;

    /// <summary>
    /// Repository with all the queries to the database using the entity framework
    /// </summary>
    internal class SeedDotnetRepository : ISeedDotnetRepository
    {
        private readonly SeedDotnetContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedDotnetRepository"/> class. 
        /// </summary>
        /// <param name="context">
        /// database context
        /// </param>
        public SeedDotnetRepository(SeedDotnetContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get the user information providing a refresh token, in this case we are using a database but you can use other system.
        /// </summary>
        /// <param name="token">token to refresh
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<UserManage> GetUserManageWithRefreshToken(string token)
        {
            return await this.context.Users.Where(t => t.RefreshToken == token).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Update the refresh token of the user session
        /// </summary>
        /// <param name="user">user to refresh the token
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UpdateRefreshToken(UserManage user)
        {
            this.context.Entry(user).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}