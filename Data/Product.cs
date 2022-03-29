using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DI_probni.Data
{
    public class Product
    {        
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }


        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
