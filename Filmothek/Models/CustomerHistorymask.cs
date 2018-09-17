using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class CustomerHistorymask
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CustomerHistorymask(int Id, string MovieName, DateTime StartDate, DateTime EndDate)
        {
            this.Id = Id;
            this.MovieName = MovieName;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
        }

        public CustomerHistorymask() { }
    }


}
