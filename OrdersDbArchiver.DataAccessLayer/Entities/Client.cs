using System.Collections.Generic;

namespace OrdersDbArchiver.DataAccessLayer.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
