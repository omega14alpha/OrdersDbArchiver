using OrdersDbArchiver.BusinessLogicLayer.EventsArgs;
using System;

namespace OrdersDbArchiver.ConsoleClient.Infrastructure
{
    internal static class Messager
    {
        public static event EventHandler<MessageEventArgs> OnGetMessage;

        public static void SendMessage(object sender, MessageEventArgs args)
        {
            OnGetMessage?.Invoke(sender, args);
        }
    }
}
