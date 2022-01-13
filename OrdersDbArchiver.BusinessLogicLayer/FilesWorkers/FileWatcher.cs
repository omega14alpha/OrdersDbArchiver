using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers
{
    public class FileWatcher : IFileWatcher
    {
        public event EventHandler<FileSystemEventArgs> OnNewFileDetected;

        private readonly FileSystemWatcher _fileWatcher;

        public FileWatcher(string observedFolderPath)
        {
            _fileWatcher = new FileSystemWatcher(observedFolderPath);
            _fileWatcher.Created += InviteOnNewFileDetected;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void InviteOnNewFileDetected(object sender, FileSystemEventArgs e)
        {
            OnNewFileDetected?.Invoke(sender, e);
        }
    }
}
