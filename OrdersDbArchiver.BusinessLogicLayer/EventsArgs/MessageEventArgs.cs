using System;

namespace OrdersDbArchiver.BusinessLogicLayer.EventsArgs
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public MessageEventArgs()
        {
        }

        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
