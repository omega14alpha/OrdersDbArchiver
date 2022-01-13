using OrdersDbArchiver.BusinessLogicLayer.EventsArgs;
using System;

namespace OrdersDbArchiver.BusinessLogicLayer.Infrastructure
{
    public static class Messager
    {
        public static event EventHandler<MessageEventArgs> OnMessage;

        public static void SendMessage(object sender, string message)
        {
            MessageEventArgs args = new MessageEventArgs() { Message = message };
            InviteOnMessage(sender, args);
        }

        private static void InviteOnMessage(object sender, MessageEventArgs args)
        {
            OnMessage?.Invoke(sender, args);
        }
    }
}
