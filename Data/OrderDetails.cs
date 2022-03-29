using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DI_probni.Data
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
