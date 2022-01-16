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

        private CancellationToken _token;

        private IGenericRepository<Order> _repository;

        public OrdersArchiver(AppConfigsModel configsModel, IFileInfoFactory fileInfoFactory, IFileWatcher fileWatcher)
        {
            _configsModel = configsModel;
            _fileInfoFactory = fileInfoFactory;
            _fileWatcher = fileWatcher;
            _fileWatcher.OnNewFileDetected += NewFileDetected;       
        }

        public void StartWork()
        {
            CheckDb();
            FileProcessing();
        }

        public void StopWork(CancellationToken token)
        {
            SendMessage(MessageTextData.OperationCancel);
            _token = token;
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
            try
            {
                var files = new List<FileNameModel>(GetFiles());
                ArchiveData(files);
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message + '\n');
            }
        }

        private IEnumerable<FileNameModel> GetFiles() =>
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker();

                if (_token.IsCancellationRequested)
                {
                    return null;
                }

                var files = worker.GetFilesPath(_configsModel);
                var models = _fileInfoFactory.CreateFileInfoModels(files, _configsModel.Folders.TargetFolder);
                return models;
            }).Result;

        private void ArchiveData(List<FileNameModel> files)
        {
            object locker = new object();
            Task[] tasks = new Task[files.Count()];    
            for (int i = 0; i < files.Count; i++)
            {
                tasks[i] = CreateDataProcessorTask(files[i], locker);
            }

            Task.WaitAll(tasks);
        }

        private Task CreateDataProcessorTask(FileNameModel file, object locker) =>
            Task.Factory.StartNew(() =>
            {
                DataProcessorUoW processor = new DataProcessorUoW(_repository, locker);
                processor.StartFileProcessing(file);
                if (_token.IsCancellationRequested)
                {
                    return;
                }

                SendMessage(string.Format(MessageTextData.FileHasBeenSaved, file.FileName));
            });

        private void SendMessage(string message) => InvokeOnMessage(this, new MessageEventArgs(message));       

        private void InvokeOnMessage(object sender, MessageEventArgs args) => OnMessage?.Invoke(sender, args);        
    }
}
