using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using OrdersDbArchiver.BusinessLogicLayer.EventsArgs;
using OrdersDbArchiver.DataAccessLayer.Repositories;
using OrdersDbArchiver.DataAccessLayer.Entities;
using OrdersDbArchiver.DataAccessLayer.Interfaces;
using OrdersDbArchiver.BusinessLogicLayer.Infrastructure.Constants;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    public class OrdersArchiver : IOrdersArchiver
    {
        public event EventHandler<MessageEventArgs> OnMessage;

        private readonly AppConfigsModel _configsModel;

        private readonly IFileInfoFactory _fileInfoFactory;

        private readonly IFileWatcher _fileWatcher;

        private readonly CancellationTokenSource _tokenSource;

        private IGenericRepository<Order> _repository;

        public OrdersArchiver(AppConfigsModel configsModel, IFileInfoFactory fileInfoFactory, IFileWatcher fileWatcher)
        {
            _configsModel = configsModel;
            _fileInfoFactory = fileInfoFactory;
            _fileWatcher = fileWatcher;
            _fileWatcher.OnNewFileDetected += NewFileDetected;
            _tokenSource = new CancellationTokenSource();            
        }

        public void StartWork()
        {
            CheckDb();
            FileProcessing();
        }

        public void StopWork()
        {
            SendMessage(MessageTextData.OperationCancel);
            _tokenSource.Cancel();
        }

        private void CheckDb()
        {
            TimerCallback timerCallback = new TimerCallback((e) => SendMessage("."));
            using Timer timer = new Timer(timerCallback, null, 100, 500);
            SendMessage(MessageTextData.CheckDb);
            _repository = new DbArchiverRepository<Order>(_configsModel.ConnectionStrings.ArhiverConnectionString);
            SendMessage(MessageTextData.StartWork);
        }

        private void NewFileDetected(object sender, FileSystemEventArgs e)
        {
            SendMessage(string.Format(MessageTextData.FileDetected, e.Name));
            FileProcessing();
        }

        private void FileProcessing()
        {
            CancellationToken token = _tokenSource.Token;
            try
            {
                var files = new List<FileNameModel>(GetFiles(token));
                ArchiveData(files, token);
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message + '\n');
            }
        }

        private IEnumerable<FileNameModel> GetFiles(CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker();

                if (token.IsCancellationRequested)
                {                    
                    return null;
                }

                var files = worker.GetFilesPath(_configsModel);
                var models = _fileInfoFactory.CreateFileInfoModels(files, _configsModel.Folders.TargetFolder);
                return models;
            }).Result;

        private void ArchiveData(List<FileNameModel> files, CancellationToken token)
        {
            object locker = new object();
            Task[] tasks = new Task[files.Count()];    
            for (int i = 0; i < files.Count; i++)
            {
                tasks[i] = CreateDataProcessorTask(files[i], locker, token);
            }

            Task.WaitAll(tasks);
        }

        private Task CreateDataProcessorTask(FileNameModel file, object locker, CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                DataProcessorUoW processor = new DataProcessorUoW(_repository, locker);
                processor.StartFileProcessing(file);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                SendMessage(string.Format(MessageTextData.FileHasBeenSaved, file.FileName));
            });

        private void SendMessage(string message) => InvokeOnMessage(this, new MessageEventArgs(message));       

        private void InvokeOnMessage(object sender, MessageEventArgs args) => OnMessage?.Invoke(sender, args);        
    }
}
