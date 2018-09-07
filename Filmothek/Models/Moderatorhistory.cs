using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class Moderatorhistory
    {
        public int Id { get; set; }
        public int ClinetId { get; set; }
        public string Activity { get; set; }
        public string Date { get; set; }
    }
}
