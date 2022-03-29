using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DI_probni.Data
{
    public class Reservation
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
