using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using OrdersDbArchiver.BusinessLogicLayer.Models;
using System;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers
{
    public class FileWatcher : IFileWatcher
    {
        public event EventHandler<FileSystemEventArgs> OnNewFileDetected;

        private readonly FileSystemWatcher _fileWatcher;

        public FileWatcher(FoldersInfoModel foldersInfoModel)
        {
            _fileWatcher = new FileSystemWatcher(foldersInfoModel.ObservedFolder);
            _fileWatcher.Filter = foldersInfoModel.ObservedFilesPattern;
            _fileWatcher.Created += InviteOnNewFileDetected;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void InviteOnNewFileDetected(object sender, FileSystemEventArgs e)
        {
            OnNewFileDetected?.Invoke(sender, e);
        }
    }
}
