using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DI_probni.Data
{
    public class Order
    {
        public int Id { get; set; }

       
        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderedOn { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
