using OrdersDbArchiver.BusinessLogicLayer.EventsArgs;
using System;

namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IOrdersArchiver
    {
        event EventHandler<MessageEventArgs> OnMessage;

        void StartWork();

        void StopWork();
    }
}
