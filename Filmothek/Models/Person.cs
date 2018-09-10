using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string Login { get; set; }
        public string Pw { get; set; }
        public short Rights { get; set; }
    }
}


