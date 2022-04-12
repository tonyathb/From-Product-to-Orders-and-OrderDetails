using DI_probni.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI_probni.Models
{
    public class ProductVM
    {
        
        [Key]
        public int Id { get; set; }

       // [Required(ErrorMessage ="Enter the product's name: ")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        //public ICollection<OrderDetails> OrderDetails { get; set; }

    }
}
