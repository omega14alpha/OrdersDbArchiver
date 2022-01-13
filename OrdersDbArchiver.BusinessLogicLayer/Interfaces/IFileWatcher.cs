using System;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IFileWatcher
    {
        event EventHandler<FileSystemEventArgs> OnNewFileDetected;
    }
}
