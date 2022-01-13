using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Infrastructure.Constants;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    internal class DataProcessor : IDataProcessor
    {
        private readonly string dataFormat = "dd.MM.yyyy";

        private readonly Guid _sessionGuid; 

        private object _locker;

        private IGenericRepository<Order> _repository;

        public DataProcessor(string connectionString, object locker)
        {
            _sessionGuid = Guid.NewGuid();    
            _locker = locker;
            lock(_locker)
            {
                _repository = new DbArchiverRepository<Order>(connectionString);
            }
        }

        public void Start(FileNameModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(string.Format(TextData.ArgumentIsNull, nameof(model)));
            }

            IFileWorker worker = new FileWorker();
            var data = worker.ReadFileData(model);

            try
            {
                var orders = GetOrdersCollection(data, model);
                _repository.AddRange(orders);
                _repository.SaveData();
                worker.FileTransfer(model);
            }
            catch (Exception ex)
            {
                _repository.Remove(s => s.SessionId == _sessionGuid);
                throw new Exception(string.Format(TextData.DbWorkError, ex.Message));
            }
        }

        private IEnumerable<Order> GetOrdersCollection(IEnumerable<string> datas, FileNameModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(string.Format(TextData.ArgumentIsNull, nameof(model)));
            }

            foreach (var data in datas)
            {
                var d = data.Split(';');
                lock (_locker)
                {
                    Order order = new Order()
                    {
                        Date = DateTime.ParseExact(d[0], dataFormat, CultureInfo.InvariantCulture),
                        AmountOfMoney = Convert.ToDouble(d[3], CultureInfo.GetCultureInfo("en-US")),
                        SessionId = _sessionGuid,
                        Manager = _repository.FindOrAdd(new Manager() { Surname = model.Manager }, s => s.Surname == model.Manager),
                        Client = _repository.FindOrAdd(new Client() { Name = d[1] }, s => s.Name == d[1]),
                        Item = _repository.FindOrAdd(new Item() { Name = d[2] }, s => s.Name == d[2])
                    };

                    yield return order;
                }
            }
        }

        public void Dispose()
        {
            _repository = null;
        }
    }
}
