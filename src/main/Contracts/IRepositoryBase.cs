namespace main.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using main.Entities.Models;

    using X.PagedList;

    /// <summary>
    ///     Interface with all the method needed
    /// </summary>
    public interface IRepositoryBase<TEntity>
        where TEntity : class
    {
        Task Add(TEntity entity);

        Task AddRange(IEnumerable<TEntity> entities);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> Get(Guid id);

        IEnumerable<TEntity> GetAll();

        Task<IPagedList<TEntity>> GetAllWithPagination(int pageIndex, int pageSize);

        int GetCountTotalItems();

        Task<UserManage> GetUserManageWithRefreshToken(string token);

        Task Remove(TEntity entity);

        Task RemoveRange(IEnumerable<TEntity> Entities);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task Update(TEntity entity);

        Task UpdateRefreshToken(UserManage user);
    }
}