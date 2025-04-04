using Microsoft.EntityFrameworkCore;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Infrastructure.Data;
using System.Linq.Expressions;

namespace PawaPayGateway.Infrastructure.Implementations
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {

        protected readonly DataContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryBase(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddAsync(List<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            await SaveChangesAsync();
            return entities;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await SaveChangesAsync();
        }

        public Task DeleteAsync(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IQueryable<T> GetByExpression(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate).AsNoTracking();
        }

        public async Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> UpdateAsync(List<T> entities)
        {
            _dbContext.Entry(entities).State = EntityState.Modified;
            await SaveChangesAsync();
            return entities;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
