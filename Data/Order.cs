using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool Final { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
