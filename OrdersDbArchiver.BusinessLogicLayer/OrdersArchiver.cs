using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Infrastructure;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer
{
    public class OrdersArchiver : IOrdersArchiver
    {
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

        public void Start()
        {
            CancellationToken token = _tokenSource.Token;
            try
            {
                var files = new List<FileNameModel>(GetFiles(token));
                ArchiveData(files, token);
            }
            catch (Exception ex)
            {
                Messager.SendMessage(this, ex.Message);
            }
        }

        public void StopWork() => _tokenSource.Cancel();        

        private void NewFileDetected(object sender, FileSystemEventArgs e) => Start();        

        private IEnumerable<FileNameModel> GetFiles(CancellationToken token) => 
            Task.Factory.StartNew(() =>
            {
                IFileWorker worker = new FileWorker();

                if (token.IsCancellationRequested)
                {
                    return null;
                }

                string message = string.Empty;

                try
                {
                    var files = worker.GetFilesPath(_configsModel);
                    var models =  _fileInfoFactory.CreateFileInfoModels(files, _configsModel.Folders.TargetFolder);
                    message = $"Detected {models.Count()} files.";
                    return models;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return null;
                }
                finally
                {
                    Messager.SendMessage(this, message);
                }
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
                using DataProcessor processor = new DataProcessor(_configsModel.ConnectionStrings.ArhiverConnectionString, locker);
                processor.Start(file);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                Messager.SendMessage(this, $"File {file.FileName} has been saved.");
            });
    }
}
