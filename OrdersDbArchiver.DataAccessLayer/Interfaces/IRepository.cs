using System;
using System.Collections.Generic;

namespace OrdersDbArchiver.DataAccessLayer.Interfaces
{
    internal interface IRepository<T>
    {
        T FindOrAdd(T entity, Func<T, bool> func);

        void AddRange(IEnumerable<T> entities);

        void Remove(Guid sessionGuid, Func<T, bool> func);

        void SaveData();
    }
}
