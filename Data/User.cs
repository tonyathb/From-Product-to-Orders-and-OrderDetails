using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DI_probni.Data
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
