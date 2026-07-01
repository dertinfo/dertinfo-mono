using DertInfo.CrossCutting.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    //The Base repository for this application. This Base Repository uses entity framework 7
    public class BaseRepository<TEntity, K, TContext> : IRepository<TEntity, K> where TEntity : Models.Database.DatabaseEntity where TContext : DbContext
    {
        protected TContext _context;
        protected DbSet<TEntity> _entitySet;
        protected IDertInfoUser _user;

        public BaseRepository(TContext context, IDertInfoUser user)
        {
            _user = user;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this._context = context;
            _entitySet = _context.Set<TEntity>();
        }

        public async Task<TEntity> Add(TEntity item)
        {
            var task = Task.Run(() =>
            {
                item.DateCreated = DateTime.Now;
                item.DateModified = DateTime.Now;
                item.CreatedBy = _user.AuthId;
                item.ModifiedBy = _user.AuthId;

                _entitySet.Add(item);
                _context.SaveChanges();
                return item;
            });

            return await task;
        }

        public async Task<IList<TEntity>> AddRange(IList<TEntity> items)
        {
            var task = Task.Run(() =>
            {
                foreach (var item in items)
                {
                    item.DateCreated = DateTime.Now;
                    item.DateModified = DateTime.Now;
                    item.CreatedBy = _user.AuthId;
                    item.ModifiedBy = _user.AuthId;
                }

                _entitySet.AddRange(items);
                _context.SaveChanges();
                return items;
            });

            return await task;
        }

        public async Task<bool> Delete(TEntity item)
        {
            var task = Task.Run(() =>
            {
                _entitySet.Attach(item);
                _entitySet.Remove(item);
                _context.SaveChanges();
                return true;
            });

            return await task;
        }

        public async Task<bool> DeleteById(K id)
        {
            var task = Task.Run(() =>
            {
                var item = _entitySet.Find(id);
                _entitySet.Remove(item);
                _context.SaveChanges();
                return true;
            });

            return await task;
        }

        public async Task<TEntity> GetById(K id)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Find(id);
            });

            return await task;
        }

        public async Task<IList<TEntity>> GetAll()
        {
            var task = Task.Run(() =>
            {
                return _entitySet.ToList();
            });

            return await task;
        }

        public async Task<IList<TEntity>> GetAll(Expression<Func<TEntity, K>> orderBy)
        {
            

            var task = Task.Run(() =>
            {
                return _entitySet.OrderBy(orderBy).ToList();
            });

            return await task;
        }

        public async Task<IList<TEntity>> GetAll(int pageIndex, int pageSize)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            });

            return await task;
        }

        public async Task<IList<TEntity>> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, K>> orderBy)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(orderBy).ToList();
            });

            return await task;
        }

        public async Task<IList<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Where(predicate).ToList();
            });

            return await task;
        }

        public async Task<IList<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, Expression<Func<TEntity, K>> orderBy)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Where(predicate).Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(orderBy).ToList();
            });

            return await task;
        }

        public async Task<bool> Update(TEntity item)
        {
            var task = Task.Run(() =>
            {
                item.DateModified = DateTime.Now;
                item.ModifiedBy = _user.AuthId;

                _entitySet.Attach(item);
                _context.SaveChanges();
                return true;
            });

            return await task;
        }

        public async Task<int> Count()
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Count();
            });

            return await task;
        }

        public async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Count(predicate);
            });

            return await task;
        }

        public async Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.SingleOrDefault(predicate);
            });

            return await task;
        }

        public async Task<IQueryable<TEntity>> Query(Expression<Func<TEntity, bool>> predicate)
        {
            var task = Task.Run(() =>
            {
                return _entitySet.Where(predicate);
            });

            return await task;
        }
    }
}
