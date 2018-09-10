using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class Customer : Person
    {        
        public List<CustomerHistory> CustomerHistory { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
