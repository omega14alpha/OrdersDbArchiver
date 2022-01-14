using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    internal class DataProcessorUoW : IDataProcessor
    {
        private readonly string dataFormat = "dd.MM.yyyy";

        private readonly Guid _sessionGuid; 

        private object _locker;

        private IGenericRepository<Order> _repository;

        private IFileWorker _worker;

        public DataProcessorUoW(string connectionString, object locker)
        {
            _sessionGuid = Guid.NewGuid();    
            _locker = locker;
            _worker = new FileWorker();
            lock (_locker)
            {
                _repository = new DbArchiverRepository<Order>(connectionString);
            }
        }

        public void StartFileProcessing(FileNameModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(model)}' cannot be equals null.");
            }

            var data = _worker.ReadFileData(model);
            var orders = GetOrdersCollection(data, model);
            SaveDataToDb(orders);
            TransferFile(model);
        }

        private void SaveDataToDb(IEnumerable<Order> orders)
        {
            try
            {                
                _repository.AddRange(orders);
                _repository.SaveData();
            }
            catch (Exception)
            {
                _repository.Remove(s => s.SessionId == _sessionGuid);
                throw new Exception($"When saving data to the database, an error occurred, data was rolled back.");
            }
        }

        private void TransferFile(FileNameModel model)
        {
            try
            {
                _worker.FileTransfer(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<Order> GetOrdersCollection(IEnumerable<string> datas, FileNameModel model)
        {
            if (datas == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(datas)}' cannot be equals null.");
            }

            if (model == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(model)}' cannot be equals null.");
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
    }
}
