namespace Main.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Main.Contracts;
    using Main.Entities;
    using Main.Entities.Models;

    using Microsoft.EntityFrameworkCore;

    using X.PagedList;

    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        protected readonly DatabaseContext context;

        /// <summary>
        ///     Set the context of the app
        /// </summary>
        /// <param name="_context"></param>
        public RepositoryBase(DatabaseContext _context)
        {
            this.context = _context;
        }

        public async Task Add(TEntity entity)
        {
            await this.context.Set<TEntity>().AddAsync(entity);
            await this.context.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<TEntity> Entities)
        {
            await this.context.Set<TEntity>().AddRangeAsync(Entities);
            await this.context.SaveChangesAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return this.context.Set<TEntity>().Where(predicate);
        }

        public async Task<TEntity> Get(Guid id)
        {
            return await this.context.Set<TEntity>().FindAsync(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.context.Set<TEntity>().ToList();
        }

        public async Task<IPagedList<TEntity>> GetAllWithPagination(int pageIndex, int pageSize)
        {
            return new PagedList<TEntity>(this.context.Set<TEntity>(), pageIndex, pageSize);
        }

        public int GetCountTotalItems()
        {
            return this.context.Set<TEntity>().Count();
        }

        /// <summary>
        ///     Get the user information providing a refresh token, in this case we are using a database but you can use other
        ///     system.
        /// </summary>
        /// <param name="token">
        ///     token to refresh
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<UserManage> GetUserManageWithRefreshToken(string token)
        {
            return await this.context.Users.Where(t => t.RefreshToken == token).FirstOrDefaultAsync();
        }

        public async Task Remove(TEntity entity)
        {
            this.context.Entry(entity).State = EntityState.Deleted;
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<TEntity> Entities)
        {
            this.context.Set<TEntity>().RemoveRange(Entities);
            await this.context.SaveChangesAsync();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return this.context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public async Task Update(TEntity entity)
        {
            this.context.Entry(entity).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        ///     Update the refresh token of the user session
        /// </summary>
        /// <param name="user">
        ///     user to refresh the token
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task UpdateRefreshToken(UserManage user)
        {
            this.context.Entry(user).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}