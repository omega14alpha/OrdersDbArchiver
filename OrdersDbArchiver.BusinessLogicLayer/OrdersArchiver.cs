using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Factories;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using OrdersDbArchiver.DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    public class OrdersArchiver
    {
        private readonly string _connectionString;

        private readonly string _observedFolder;

        private readonly IFileInfoFactory _fileInfoFactory;

        public OrdersArchiver(string connectionString, string observedFolder, string targetFileFolder)
        {
            _connectionString = connectionString;
            _observedFolder = observedFolder;

            _fileInfoFactory = new FileInfoFactory(targetFileFolder);
        }

        public void Start()
        {
            CheckOrCreateDb();
            var files = new List<OrderFileName>(GetFiles());
            ArchiveData(files);
        }

        private void CheckOrCreateDb() => 
            Task.Factory.StartNew(() =>
            {
                IGenericRepository<Order> _context = new DbArchiverRepository<Order>(_connectionString);
            }).Wait();

        private IEnumerable<OrderFileName> GetFiles() => 
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker(); 
                return worker.GetFilesPath(_observedFolder, "*.csv", _fileInfoFactory);
            }).Result;

        private void ArchiveData(List<OrderFileName> files)
        {
            object locker = new object();
            Task[] tasks = new Task[files.Count()];
            for (int i = 0; i < files.Count; i++)
            {
                tasks[i] = CreateTask(files[i], locker);
            }
            
            Task.WaitAll(tasks);
        }

        private Task CreateTask(OrderFileName file, object locker) =>
            Task.Factory.StartNew(() =>
            {
                DataProcessor processor = new DataProcessor(_connectionString, locker);
                processor.Start(file);
            });
    }
}
