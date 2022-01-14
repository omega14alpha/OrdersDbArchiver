using OrdersDbArchiver.BusinessLogicLayer.Models;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    internal interface IDataProcessor
    {
        void StartFileProcessing(FileNameModel file);
    }
}
