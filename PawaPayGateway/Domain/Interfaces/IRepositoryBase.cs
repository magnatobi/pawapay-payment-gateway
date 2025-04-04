using System.Linq.Expressions;

namespace PawaPayGateway.Domain.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetByExpression(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddAsync(List<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateAsync(List<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteAsync(List<T> entities);
        Task SaveChangesAsync();
    }
}
