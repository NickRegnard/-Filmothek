using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class CustomerHistory
    {
        public int Id { get; set; }
        public int MovieId {get; set;}
        public int CustomerId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string Note { get; set; }
        public bool isBorrowing { get; set; }
    }
}
