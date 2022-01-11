using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    internal class DataProcessor
    {
        private readonly Guid _sessionGuid;

        private object _locker;

        private readonly IGenericRepository<Order> _repository;

        public DataProcessor(string connectionString, object locker)
        {
            _sessionGuid = Guid.NewGuid();
            _repository = new DbArchiverRepository<Order>(connectionString);           
            _locker = locker;
        }

        public void Start(OrderFileName file)
        {
            IFileWorker worker = new FileWorker();
            var data = worker.ReadFileData(file);
            var orders = GetOrdersCollection(data, file.Manager);

            try
            {
                _repository.AddRange(orders);
                _repository.SaveData();
                worker.FileTransfer(file);
            }
            catch (Exception)
            {
                _repository.Remove(s => s.SessionId == _sessionGuid);
            }
        }

        private IEnumerable<Order> GetOrdersCollection(IEnumerable<string> datas, string man)
        {
            foreach (var data in datas)
            {
                var d = data.Split(';');
                lock (_locker)
                {
                    Order order = new Order()
                    {
                        Date = DateTime.ParseExact(d[0], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        AmountOfMoney = Convert.ToDouble(d[3], CultureInfo.GetCultureInfo("en-US")),
                        SessionId = _sessionGuid,
                        Manager = _repository.FindOrAdd(new Manager() { Surname = man }, s => s.Surname == man),
                        Client = _repository.FindOrAdd(new Client() { Name = d[1] }, s => s.Name == d[1]),
                        Item = _repository.FindOrAdd(new Item() { Name = d[2] }, s => s.Name == d[2])
                    };

                    yield return order;
                }
            }
        }
    }
}
