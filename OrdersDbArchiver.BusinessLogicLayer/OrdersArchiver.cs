using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Factories;
using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    public class OrdersArchiver
    {
        private readonly string _connectionString;

        private readonly string _observedFolder;

        private readonly IFileInfoFactory _fileInfoFactory;

        private readonly CancellationTokenSource _tokenSource;

        public OrdersArchiver(string connectionString, string observedFolder, string targetFileFolder)
        {
            _connectionString = connectionString;
            _observedFolder = observedFolder;

            _fileInfoFactory = new FileInfoFactory(targetFileFolder);
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            CancellationToken token = _tokenSource.Token;
            var files = new List<OrderFileName>(GetFiles(token));
            ArchiveData(files, token);
        }

        public void StopWork()
        {
            _tokenSource.Cancel();
        }

        private IEnumerable<OrderFileName> GetFiles(CancellationToken token) => 
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker();

                if (token.IsCancellationRequested)
                {
                    return null;
                }

                return worker.GetFilesPath(_observedFolder, "*.csv", _fileInfoFactory);
            }).Result;

        private void ArchiveData(List<OrderFileName> files, CancellationToken token)
        {
            object locker = new object();
            Task[] tasks = new Task[files.Count()];
            for (int i = 0; i < files.Count; i++)
            {
                tasks[i] = CreateTask(files[i], locker, token);
            }
            
            Task.WaitAll(tasks);
        }

        private Task CreateTask(OrderFileName file, object locker, CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                DataProcessor processor = new DataProcessor(_connectionString, locker);
                processor.Start(file);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            });
    }
}
