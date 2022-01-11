using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IFileInfoFactory
    {
        OrderFileName CreateModel(string fileName);
    }
}
