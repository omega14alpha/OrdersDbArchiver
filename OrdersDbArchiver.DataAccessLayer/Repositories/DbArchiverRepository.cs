using OrdersDbArchiver.DataAccessLayer.Contexts;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrdersDbArchiver.DataAccessLayer.Repositories
{
    public class DbArchiverRepository<T> : IRepository<T> where T : class
    {
        private readonly DbArchiverContext _context;

        public DbArchiverRepository(string connectionString)
        {
            _context = new DbArchiverContext(connectionString);
        }

        public T FindOrAdd(T entity, Func<T, bool> func)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "");
            }

            return _context.Set<T>().Where(func)?.FirstOrDefault() ?? _context.Set<T>().Add(entity);
        } 

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Remove(Guid sessionGuid, Func<T, bool> func)
        {
            if (sessionGuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(sessionGuid), "");
            }

            T item = _context.Set<T>().Where(func)?.FirstOrDefault();
            if (item != null)
            {
                _context.Set<T>().Remove(item);
            }
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
