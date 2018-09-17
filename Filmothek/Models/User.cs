using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class User
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string Login { get; set; }

        public User() { }
        public User(Customer Customer)
        {
            this.Id = Customer.Id;
            this.LastName = Customer.LastName;
            this.FirstName = Customer.FirstName;
            this.Address = Customer.Address;
            this.Login = Customer.Login;
        }
    }
}
