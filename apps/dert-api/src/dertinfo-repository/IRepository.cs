using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IRepository<T, K> where T : class
    {
        Task<T> Add(T item);

        Task<IList<T>> AddRange(IList<T> items);

        Task<bool> Update(T item);

        Task<bool> DeleteById(K id);

        Task<bool> Delete(T item);

        Task<T> GetById(K id);

        Task<IList<T>> GetAll();

        Task<IList<T>> GetAll(Expression<Func<T, K>> orderBy);

        Task<IList<T>> GetAll(int pageIndex, int pageSize);

        Task<IList<T>> GetAll(int pageIndex, int pageSize, Expression<Func<T, K>> orderBy);

        Task<IList<T>> Find(Expression<Func<T, bool>> predicate);

        Task<IList<T>> Find(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, Expression<Func<T, K>> orderBy);

        Task<int> Count();

        Task<int> Count(Expression<Func<T, bool>> predicate);

        Task<T> SingleOrDefault(Expression<Func<T, bool>> predicate);

        Task<IQueryable<T>> Query(Expression<Func<T, bool>> predicate);
    }
}
