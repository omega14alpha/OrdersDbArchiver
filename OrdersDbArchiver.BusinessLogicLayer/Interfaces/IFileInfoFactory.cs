using OrdersDbArchiver.BusinessLogicLayer.Models;
using System.Collections.Generic;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IFileInfoFactory
    {
        IEnumerable<FileNameModel> CreateFileInfoModels(IEnumerable<string> filePaths, string targetPath);
    }
}
