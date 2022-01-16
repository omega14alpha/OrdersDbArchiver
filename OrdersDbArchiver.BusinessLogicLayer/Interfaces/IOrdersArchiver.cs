using OrdersDbArchiver.BusinessLogicLayer.EventsArgs;
using System;
using System.Threading;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IOrdersArchiver
    {
        event EventHandler<MessageEventArgs> OnMessage;

        void StartWork();

        void StopWork(CancellationToken token);
    }
}
