using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using System.Collections.Generic;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    internal interface IFileWorker
    {
        IEnumerable<OrderFileName> GetFilesPath(string folderPath, string searchFileExtension, IFileInfoFactory fileInfoFactory);

        IEnumerable<string> ReadFileData(OrderFileName fileNameModel);

        void FileTransfer(OrderFileName fileNameModel);
    }
}
