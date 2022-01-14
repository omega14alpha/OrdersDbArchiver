using OrdersDbArchiver.DataAccessLayer.Contexts;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrdersDbArchiver.DataAccessLayer.Repositories
{
    public class DbArchiverRepository<T> : IGenericRepository<T> where T : class
    {
        private DbArchiverContext _context;

        public DbArchiverRepository(string connectionString)
        {
            _context = new DbArchiverContext(connectionString);
        }

        public E FindOrAdd<E>(E entity, Func<E, bool> func) where E : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(entity)}' cannot be equals null.");
            }

            var result = _context.Set<E>().Where(func).FirstOrDefault();
            if (result == null)
            {
                result = _context.Set<E>().Add(entity);
                SaveData();
            }

            return result;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(entities)}' cannot be equals null.");
            }

            _context.Set<T>().AddRange(entities);
        }

        public void Remove(Func<T, bool> func)
        {
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
