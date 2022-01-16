using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using OrdersDbArchiver.BusinessLogicLayer.Infrastructure.Constants;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    internal class DataProcessorUoW : IDataProcessor
    {
        private const string dataFormat = "dd.MM.yyyy";

        private readonly Guid _sessionGuid; 

        private object _locker;

        private readonly IGenericRepository<Order> _repository;

        private readonly IFileWorker _worker;

        public DataProcessorUoW(IGenericRepository<Order> repository, object locker)
        {
            _sessionGuid = Guid.NewGuid();    
            _locker = locker;
            _worker = new FileWorker();
            _repository = repository;
        }

        public void StartFileProcessing(FileNameModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(string.Format(ErrorTextData.ArgumentCannotBeNull, nameof(model)));
            }

            var data = _worker.ReadFileData(model);
            var orders = GetOrders(data, model);
            
            lock(_locker)
            {
                SaveDataToDb(orders);
            }

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
                throw new Exception(ErrorTextData.SavingErrorAndDataRolledBack);
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
                throw new Exception(ErrorTextData.TransferringFileError); 
            }
        }

        private IEnumerable<Order> GetOrders(IEnumerable<string> datas, FileNameModel model)
        {
            if (datas == null)
            {
                throw new ArgumentNullException(string.Format(ErrorTextData.ArgumentCannotBeNull, nameof(datas)));
            }

            if (model == null)
            {
                throw new ArgumentNullException(string.Format(ErrorTextData.ArgumentCannotBeNull, nameof(model)));
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
