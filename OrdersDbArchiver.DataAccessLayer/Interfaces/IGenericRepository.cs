using System;
using System.Collections.Generic;

namespace OrdersDbArchiver.DataAccessLayer.Interfaces
{
    public interface IGenericRepository<T>
    {
        E FindOrAdd<E>(E entity, Func<E, bool> func) where E : class;

        void AddRange(IEnumerable<T> entities);

        void Remove(Func<T, bool> func);

        void SaveData();
    }
}
