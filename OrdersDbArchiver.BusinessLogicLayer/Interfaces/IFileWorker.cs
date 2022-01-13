using OrdersDbArchiver.BusinessLogicLayer.Models;
using System.Collections.Generic;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    internal interface IFileWorker
    {
        IEnumerable<string> GetFilesPath(AppConfigsModel configsModel);

        IEnumerable<string> ReadFileData(FileNameModel fileNameModel);

        void FileTransfer(FileNameModel fileNameModel);
    }
}
