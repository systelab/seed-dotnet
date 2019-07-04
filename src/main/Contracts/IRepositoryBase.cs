namespace main.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Entities.Models;
    using PagedList.Core;

    /// <summary>
    ///     Interface with all the method needed
    /// </summary>
    public interface IRepositoryBase<TEntity> where TEntity
        : class
    {
        Task<TEntity> Get(Guid id);
        IEnumerable<TEntity> GetAll();
        Task<PagedList<TEntity>> GetAllWithPagination(int pageIndex, int pageSize);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task Add(TEntity entity);
        Task AddRange(IEnumerable<TEntity> Entities);

        Task Remove(TEntity entity);
        Task RemoveRange(IEnumerable<TEntity> Entities);

        Task Update(TEntity entity);

        int GetCountTotalItems();

        Task<UserManage> GetUserManageWithRefreshToken(string token);

        Task UpdateRefreshToken(UserManage user);
    }
}