using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace AuthService.Domain.Interface
{
    public interface IRepository<TEntity, TKey>where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity?> GetAsync(TKey id);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string? includeProperties = null);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
