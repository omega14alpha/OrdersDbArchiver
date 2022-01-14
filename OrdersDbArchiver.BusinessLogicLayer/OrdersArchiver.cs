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

namespace OrdersDbArchiver.BusinessLogicLayer
{
    public class OrdersArchiver : IOrdersArchiver
    {
        public event EventHandler<MessageEventArgs> OnMessage;

        private readonly AppConfigsModel _configsModel;

        private readonly IFileInfoFactory _fileInfoFactory;

        private readonly IFileWatcher _fileWatcher;

        private readonly CancellationTokenSource _tokenSource;

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
            CancellationToken token = _tokenSource.Token;
            try
            {
                var files = new List<FileNameModel>(GetFiles(token));
                SendMessage($"Founded {files.Count} files.");
                ArchiveData(files, token);
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
            }
        }

        public void StopWork() => _tokenSource.Cancel();

        private void NewFileDetected(object sender, FileSystemEventArgs e)
        {
            SendMessage($"Detected file -> {e.Name}.");
            StartWork();
        }

        private IEnumerable<FileNameModel> GetFiles(CancellationToken token) =>
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker();

                if (token.IsCancellationRequested)
                {
                    SendMessage($"Operation canceled.");
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
                DataProcessorUoW processor = new DataProcessorUoW(_configsModel.ConnectionStrings.ArhiverConnectionString, locker);
                processor.StartFileProcessing(file);
                if (token.IsCancellationRequested)
                {
                    SendMessage($"Operation canceled.");
                    return;
                }

                SendMessage($"File {file.FileName} has been saved.");
            });

        private void SendMessage(string message)
        {
            InvokeOnMessage(this, new MessageEventArgs(message));
        }

        private void InvokeOnMessage(object sender, MessageEventArgs args)
        {
            OnMessage?.Invoke(sender, args);
        }
    }
}
