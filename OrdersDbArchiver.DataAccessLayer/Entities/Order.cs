using System;

namespace OrdersDbArchiver.DataAccessLayer.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ManagerId { get; set; }
        public Manager Manager { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public double AmountOfMoney { get; set; }

        public Guid SessionId { get; set; }
    }
}
