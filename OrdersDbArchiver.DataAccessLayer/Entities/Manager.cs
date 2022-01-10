using System.Collections.Generic;

namespace OrdersDbArchiver.DataAccessLayer.Entities
{
    public class Manager
    {
        public int Id { get; set; }

        public string Surname { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
