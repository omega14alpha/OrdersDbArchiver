using OrdersDbArchiver.BusinessLogicLayer.Models;
using System;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    internal interface IDataProcessor : IDisposable
    {
        void Start(FileNameModel file);
    }
}
